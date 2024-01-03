using WhimsicalWeather.Behaviours;
using WhimsicalWeather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WhimsicalWeather.Controllers
{
    internal class ReplacementController
    {
        private static List<string> _imageSpriteNames = new()
        {
            "DialogueBox1Frame 1", // used for chat menu, inventory slots, etc
            "DialogueBox1Frame 2", 
            "DialogueBox1Frame 3", 
            "DialogueBox1Frame 4",
            "DialogueBox1Frame 5", // not 100% sure what 5 and 6 are used for
            "DialogueBox1Frame 6",
            "SprintMeter",
            "scanCircle1",
            "scanCircle2",
            "ScanUI1",
            "DialogueBoxSimple",
            "BatteryIcon",
            "BatteryMeter",
            "BatteryMeterFrame",
            "timeIcon1",
            "timeIcon2", 
            "timeIcon3",
            "timeIcon3b",
            "SpeakingSymbol", // spectate voice chat activity
            "CircleMeterOutline", // interaction circle
            "CircleMeterFill",
            "endgameStatsBG", // endgame stats background
            "endgameStatsBoxes",
            "endgameAllPlayersDead",
            "shipClockIcon"
        };

        // use "/" to denote parent gameObject
        private static List<string> _imageGameObjectNames = new()
        {
            "HeaderTextBox", // scan node name
            "SubTextBox", // scan node price background
            "Panel (2)", // pause menu background
            "Panel (1)", // pause menu outer decoration
            "PleaseConfirmChangesPanel/Panel",
            "Symbol", // endgame stats "deceased"/checkmark image 
            "DividingLine", // used in profitquota/endgamestats/rewardsscreen 
            "Container2/Image", // rewards screen background
            "LevelUp/Image", // level up upper background
            "LevelUp/Image (2)", // level up background
            "LevelUp/LevelUpMeter", // level up meter. this may be able to use the sprite name, but i'm not sure if it's used elsewhere
            "Penalty/Image"
        };

        private static List<string> _particleMaterialNames = new()
        {
            "SparkParticle", // zapgun sparks
            "BeamUpZap", // teleporter
            "BeamUpZap 1" // inverse teleporter
        };

        private List<MaterialReplacement> _materialReplacements = new();

        private List<ImageShaderReplacement> _imageShaderReplacements = new();

        // includes all sound replacements, including playoneshot
        private List<SoundReplacementBehaviour> _soundReplacements = new();

        // cached to not use linq every time a sound plays
        private List<SoundReplacementBehaviour> _oneShotSoundReplacements = new();

        private AudioClip? _highAction1Clip;

        public void CreateSoundReplacements(List<SoundReplacementBehaviour> soundReplacements)
        {
            _soundReplacements = soundReplacements;
            _oneShotSoundReplacements = _soundReplacements.Where(x => x.SoundReplacementType == SoundReplacementType.OneShotAudio).ToList();
        }

        public void CreateTextureReplacements(List<TextureReplacementBehaviour> textureReplacements)
        {
            foreach (var replacement in textureReplacements)
            {
                _materialReplacements.Add(new TextureReplacement(replacement));
            }
            foreach (var particleMaterialName in _particleMaterialNames)
            {
                if (particleMaterialName == "SparkParticle" && !Config.Instance.RainbowZapGun.Value) continue;
                _materialReplacements.Add(new MaterialNameShaderReplacement(Plugin.RainbowParticleShader, particleMaterialName, true));
            }
            if (Config.Instance.RainbowItemDropship.Value)
            {
                _materialReplacements.Add(new MaterialNameShaderReplacement(Plugin.RainbowDropshipLightsShader, "LEDLightYellow", false)); // item dropship lights
                _materialReplacements.Add(new ColorReplacement(new Color(0.9f, 0, 0, 1), "LEDLightYellow", "_Color")); // make item dropship lights more vibrant
            }
            if (Config.Instance.ClearMagnifyingGlass.Value)
            {
                _materialReplacements.Add(new ColorReplacement(new Color(0, 0, 0.2f, 0.3f), "GlassTex", "_BaseColor")); // make magnifying glass more clear
                _materialReplacements.Add(new FloatReplacement(0.85f, "GlassTex", "_Smoothness")); // make magnifying glass less blurry
            }
            if (Config.Instance.RainbowApparatus.Value)
            {
                _materialReplacements.Add(new MaterialNameShaderReplacement(Plugin.RainbowUIShader, "TankLight", false)); // apparatus light
                _materialReplacements.Add(new ColorReplacement(new Color(0.8f, 0, 0, 1), "TankLight", "_Color")); // apparatus light brightness
                _materialReplacements.Add(new FloatReplacement(5f, "TankLight", "_HueShiftSpeed")); // apparatus light speed
            }
        }

        public void CreateImageShaderReplacements()
        {
            if (!Config.Instance.RainbowUI.Value) return;
            foreach (var imageSpriteName in _imageSpriteNames)
            {
                var replacement = new ImageNameShaderReplacement(Plugin.RainbowUIShader, imageSpriteName);
                _imageShaderReplacements.Add(replacement);
            }
            foreach (var imageGameObjectName in _imageGameObjectNames)
            {
                ImageGameObjectNameShaderReplacement replacement;
                var split = imageGameObjectName.Split('/');
                if (split.Length > 1) replacement = new(Plugin.RainbowUIShader, split[1], split[0]);
                else replacement = new(Plugin.RainbowUIShader, imageGameObjectName);

                _imageShaderReplacements.Add(replacement);
            }
            _materialReplacements.Add(new MaterialShaderReplacement(Plugin.RainbowTextShader, "TextMeshPro/Distance Field"));
        }

        // this does NOT perform well right now!
        public void ReplaceAll()
        {
            Material[] materials = Resources.FindObjectsOfTypeAll(typeof(Material)) as Material[];
            foreach (var material in materials)
            {
                foreach (var materialReplacement in _materialReplacements)
                {
                    materialReplacement.ReplaceMaterialIfMatch(material);
                }
            }

            Image[] images = Resources.FindObjectsOfTypeAll(typeof(Image)) as Image[];
            foreach (var image in images)
            {
                // TODO: If there's no sprite, we can just use the vertex color to save a ton of performance
                // Come up with second vertex-only shader
                foreach (var imageReplacement in _imageShaderReplacements)
                {
                    imageReplacement.ReplaceImageShaderIfMatch(image);
                }
            }

            foreach (var soundReplacement in _soundReplacements)
            {
                if (soundReplacement.SoundReplacementType == SoundReplacementType.SoundManagerHighAction1 && SoundManager.Instance != null)
                {
                    _highAction1Clip = SoundManager.Instance.highAction1.clip;
                    SoundManager.Instance.highAction1.clip = soundReplacement.ReplacementClip;
                    if (!GameNetworkManager.Instance.localPlayerController.isPlayerDead)
                    {
                        SoundManager.Instance.highAction1.Stop();
                        SoundManager.Instance.highAction1.Play();
                    }
                }
            }
        }

        public void UnreplaceAll()
        {
            foreach (var materialReplacement in _materialReplacements)
            {
                materialReplacement.RestoreMaterials();
            }
            foreach (var imageShaderReplacement in _imageShaderReplacements)
            {
                imageShaderReplacement.RestoreImageShaders();
            }

            foreach (var soundReplacement in _soundReplacements)
            {
                if (soundReplacement.SoundReplacementType == SoundReplacementType.SoundManagerHighAction1 && SoundManager.Instance != null)
                {
                    SoundManager.Instance.highAction1.clip = _highAction1Clip;
                    _highAction1Clip = null;
                    if (!GameNetworkManager.Instance.localPlayerController.isPlayerDead)
                    {
                        SoundManager.Instance.highAction1.Stop();
                        SoundManager.Instance.highAction1.Play();
                    }
                }
            }
        }

        private void ReplaceCustomSounds(bool enabled)
        {
            foreach (var soundReplacement in _soundReplacements)
            {
                if (soundReplacement.SoundReplacementType == SoundReplacementType.SoundManagerHighAction1 && Config.Instance.FearLaughing.Value && SoundManager.Instance != null)
                {
                    if (enabled)
                    {
                        _highAction1Clip = SoundManager.Instance.highAction1.clip;
                        SoundManager.Instance.highAction1.clip = soundReplacement.ReplacementClip;
                    }
                    else
                    {
                        SoundManager.Instance.highAction1.clip = _highAction1Clip;
                        _highAction1Clip = null;
                    }
                    if (!GameNetworkManager.Instance.localPlayerController.isPlayerDead)
                    {
                        SoundManager.Instance.highAction1.Stop();
                        SoundManager.Instance.highAction1.Play();
                    }
                }
            }
        }

        public void ReplaceSoundIfNeeded(ref AudioClip clip)
        {
            if (clip == null) return;
            string clipName = clip.name;
            var newClip = _oneShotSoundReplacements.FirstOrDefault(x => x.SoundName == clipName);

            if (newClip == null) return;
            clip = newClip.ReplacementClip;
        }
    }
}
