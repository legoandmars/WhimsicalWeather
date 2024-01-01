using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using System.Linq;
using LethalVision.Patches;
using LethalVision.Controllers;
using LethalVision.Behaviours;

namespace LethalVision.Visuals
{
    public class LethalVisuals : MonoBehaviour
    {
        private const float _animationLength = 4f;

        public static bool LethalVisualsEnabled = false;
        
        public static event Action OnVisualsEnabled;
        public static event Action OnVisualsDisabled;

        private bool _enabled = false;
        private GameObject? _sparkles;
        private GameObject? _rainbow;
        private AudioClip _sparkleAudio;

        private bool _animating = false;
        private bool _reversedAnimation;
        private float _animationProgress;

        private CustomPassVolume? _customPassVolume;
        private ReplacementController _replacementController = new();

        private void Awake()
        {
            var textureReplacementBehaviours = Plugin.VisualsObject.GetComponentsInChildren<TextureReplacementBehaviour>(true).ToList(); // load serialized texture replacements
            var soundReplacementBehaviours = Plugin.VisualsObject.GetComponentsInChildren<SoundReplacementBehaviour>(true).ToList(); // load serialized texture replacements
            _replacementController.CreateTextureReplacements(textureReplacementBehaviours);
            _replacementController.CreateSoundReplacements(soundReplacementBehaviours);
            _replacementController.CreateImageShaderReplacements();
            AudioPatches.SetReplacementController(_replacementController);

            OnVisualsEnabled += Enable;
            OnVisualsDisabled += Disable;
        }

        private void OnDestroy()
        {
            OnVisualsEnabled -= Enable;
            OnVisualsDisabled -= Disable;
        }

        private void Update()
        {
            if (BepInEx.UnityInput.Current.GetKeyDown(KeyCode.F10))
            {
                ToggleVisualsEvent(!_enabled);
            }
            // animation time is like 5 seconds i guess
            if (_animating)
            {
                SetEffectPercentage(Mathf.Clamp01(_reversedAnimation ? 1 - (_animationProgress / _animationLength) : _animationProgress / _animationLength));

                if (_animationProgress >= _animationLength)
                {
                    _animating = false;
                    if (_sparkles != null)
                    {
                        _sparkles.GetComponent<ParticleSystem>().Stop();
                    }
                    if (!_reversedAnimation)
                    {
                        ReplaceVisuals();
                    }
                    else
                    {
                        DisableAfterAnimation();
                    }
                }

                _animationProgress += Time.deltaTime;
            }
        }

        public static void ToggleVisualsEvent(bool enabled)
        {
            if (enabled) OnVisualsEnabled?.Invoke();
            else OnVisualsDisabled?.Invoke();
        }

        private void ReplaceVisuals()
        {
            _replacementController.ReplaceAll();
            WalkieTalkiePatches.SetWalkieLightIntensity(WalkieTalkiePatches.WalkieTalkieLightIntensityOverride);
            ShovelPatches.ReplaceAllShovels();
        }

        private void UnreplaceVisuals()
        {
            if (_rainbow != null)
            {
                _rainbow.SetActive(false);
            }

            _replacementController.UnreplaceAll();
            WalkieTalkiePatches.SetWalkieLightIntensity(WalkieTalkiePatches.WalkieTalkieLightIntensityDefault);
            ShovelPatches.RestoreAllShovels();
        }

        public void SetAudio(AudioClip sparkleAudio)
        {
            _sparkleAudio = sparkleAudio;
        }

        public void Enable()
        {
            Debug.Log("Enabling LethalVisuals.");
            _enabled = true;
            LethalVisualsEnabled = true;

            var localPlayer = StartOfRound.Instance?.localPlayerController;
            if (localPlayer == null) return;
            var customPass = GetCustomPass(localPlayer.gameplayCamera);
            if (customPass == null) return;

            if (_sparkles == null)
            {
                // too lazy to set up rendering priority, if the player faces a wall or something they will have to Deal With It
                _sparkles = Instantiate(Plugin.SparkParticles);
                _sparkles.transform.SetParent(localPlayer.gameplayCamera.transform, false);
                _sparkles.transform.localPosition = new Vector3(0f, 0f, 0.5f);
                _sparkles.transform.localRotation = Quaternion.identity;
                _sparkles.transform.localScale = Vector3.one;
            }

            if (_rainbow == null)
            {
                _rainbow = Instantiate(Plugin.Rainbow);
                _rainbow.transform.localPosition = new Vector3(0f, 0f, 0f);
                _rainbow.transform.localRotation = Quaternion.identity;
                _rainbow.transform.localScale = new Vector3(300f, 300f, 300f);
            }

            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.DisplayTip("Photosensitivity alert!", "<size=16px>You've landed in <color=#FF00FF>Whimsical</color> weather.\nThis mod features flashing lights that may potentially trigger seizures for people with epilepsy.", true, true, "LC_PhotosensitiveTip");
            }

            _sparkles.SetActive(true);
            _rainbow.SetActive(true);

            // Setup for animation
            PlaySparkleAudio();
            SetEffectPercentage(0f);
            _animating = true;
            _reversedAnimation = false;
            _animationProgress = 0;

            customPass.enabled = true;
        }
        public void Disable()
        {
            Debug.Log("Disabling LethalVisuals.");
            _enabled = false;
            LethalVisualsEnabled = false;

            // TODO: Add check if we're even allowed to disable here
            // TODO: This isn't getting reset on game leave
            if (_sparkles != null)
            {
                PlaySparkleAudio();
                _sparkles.GetComponent<ParticleSystem>().Play();
                SetEffectPercentage(1f);
            }

            _animating = true;
            _reversedAnimation = true;
            _animationProgress = 0;

            UnreplaceVisuals();
        }
        private void SetEffectPercentage(float progress)
        {
            Plugin.PassMaterial.SetFloat("_UVPercent", progress);
            if (_sparkles != null)
            {
                // 0.35 range
                float range = 0.33f;
                float sparklesPosition = ((1 - progress) * (range * 2)) - range;
                _sparkles.transform.localPosition = new Vector3(_sparkles.transform.localPosition.x, sparklesPosition, _sparkles.transform.localPosition.z);
            }
            VoicePitchPatches.SetVoicePitchOverridePercent(progress);
        }

        private void PlaySparkleAudio()
        {
            var localPlayer = StartOfRound.Instance?.localPlayerController;
            if (localPlayer == null) return;
            localPlayer.itemAudio.PlayOneShot(_sparkleAudio);
        }

        public void DisableAfterAnimation()
        {
            var localPlayer = StartOfRound.Instance?.localPlayerController;
            if (localPlayer == null) return;
            var customPass = GetCustomPass(localPlayer.gameplayCamera);
            if (customPass == null) return;

            if (_sparkles != null)
            {
                _sparkles.SetActive(false);
            }
            customPass.enabled = false;
        }

        private FullScreenCustomPass? GetCustomPass(Camera camera)
        {
            if (_customPassVolume != null)
            {
                // may break when null idk
                return _customPassVolume.customPasses.Where(x => x is FullScreenCustomPass customPass).FirstOrDefault() as FullScreenCustomPass;
            }

            _customPassVolume = FindObjectOfType<CustomPassVolume>();
            if (_customPassVolume == null) return null;

            var pass = _customPassVolume.AddPassOfType<FullScreenCustomPass>() as FullScreenCustomPass;
            pass.fullscreenPassMaterial = Plugin.PassMaterial;
            pass.enabled = false;

            // _customPassVolume.customPasses.Reverse();
            return pass;
        }
    }
}
