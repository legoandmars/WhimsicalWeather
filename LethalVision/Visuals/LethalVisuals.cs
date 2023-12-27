using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using System.Linq;

namespace LethalVision.Visuals
{
    public class LethalVisuals : MonoBehaviour
    {
        private const float _animationLength = 3f;

        private bool _enabled = false;
        private GameObject? _sparkles;
        private GameObject? _rainbow;
        private AudioClip _sparkleAudio;

        private bool _animating = false;
        private bool _reversedAnimation;
        private float _animationProgress;

        private CustomPassVolume? _customPassVolume;

        private void Update()
        {
            if (BepInEx.UnityInput.Current.GetKeyDown(KeyCode.P))
            {
                ToggleVisuals();
            }
            // animation time is like 5 seconds i guess
            if (_animating && _animationProgress < _animationLength)
            {
                SetEffectPercentage(_reversedAnimation ? 1 - (_animationProgress / _animationLength) : _animationProgress / _animationLength);

                _animationProgress += Time.deltaTime;
                if (_animationProgress >= _animationLength)
                {
                    _animating = false;
                    _sparkles.GetComponent<ParticleSystem>().Stop();
                    if (_reversedAnimation) DisableAfterAnimation();
                }
            }
        }

        public void SetAudio(AudioClip sparkleAudio)
        {
            _sparkleAudio = sparkleAudio;
        }

        public void ToggleVisuals()
        {
            if (_enabled)
            {
                Disable();
            }
            else
            {
                Enable();
            }
            _enabled = !_enabled;
        }

        public void Enable()
        {
            var localPlayer = StartOfRound.Instance?.localPlayerController;
            if (localPlayer == null) return;
            var customPass = GetCustomPass(localPlayer.gameplayCamera);
            if (customPass == null) return;

            if (_sparkles == null)
            {
                // too lazy to set up rendering priority, if the player faces a wall or something they will have to Deal With It
                _sparkles = Instantiate(Plugin.VisualsObject.transform.Find("Sparticles").gameObject);
                _sparkles.transform.SetParent(localPlayer.gameplayCamera.transform, false);
                _sparkles.transform.localPosition = new Vector3(0f, 0f, 0.5f);
                _sparkles.transform.localRotation = Quaternion.identity;
                _sparkles.transform.localScale = Vector3.one;
            }

            if (_rainbow == null)
            {
                _rainbow = Instantiate(Plugin.VisualsObject.transform.Find("rainbow").gameObject);
                _rainbow.transform.localPosition = new Vector3(0f, 0f, 0f);
                _rainbow.transform.localRotation = Quaternion.identity;
                _rainbow.transform.localScale = new Vector3(300f, 300f, 300f);
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
            // TODO: Add check if we're even allowed to disable here
            PlaySparkleAudio();
            _sparkles.GetComponent<ParticleSystem>().Play();
            SetEffectPercentage(1f);
            _animating = true;
            _reversedAnimation = true;
            _animationProgress = 0;
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
        }

        private void PlaySparkleAudio()
        {
            var localPlayer = StartOfRound.Instance?.localPlayerController;
            if (localPlayer == null) return;
            Debug.Log("AUDIO:");
            Debug.Log(_sparkleAudio.name);
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
            if (_rainbow != null)
            {
                _rainbow.SetActive(false);
            }
            customPass.enabled = false;
        }

        private FullScreenCustomPass? GetCustomPass(Camera camera)
        {
            if (_customPassVolume != null)
            {
                // may break when null idk
                return _customPassVolume.customPasses.Where(x => x is FullScreenCustomPass).FirstOrDefault() as FullScreenCustomPass;
            }

            _customPassVolume = FindObjectOfType<CustomPassVolume>();
            if (_customPassVolume == null) return null;

            var fullscreenPass = _customPassVolume.AddPassOfType<FullScreenCustomPass>() as FullScreenCustomPass;
            fullscreenPass.fullscreenPassMaterial = Plugin.PassMaterial;
            fullscreenPass.enabled = false;
            _customPassVolume.customPasses.Reverse();
            return fullscreenPass;
        }
    }
}
