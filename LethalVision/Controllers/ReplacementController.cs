using LethalVision.Behaviours;
using LethalVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LethalVision.Controllers
{
    internal class ReplacementController
    {
        private static List<string> _imageSpriteNames = new()
        {
            "DialogueBox1Frame 2", // used for chat menu, inventory slots, etc
            "DialogueBox1Frame 3", // also used for chat menu maybe?
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
            "SpeakingSymbol" // spectate voice chat activity
        };

        // use "/" to denote parent gameObject
        private static List<string> _imageGameObjectNames = new()
        {
            "HeaderTextBox", // scan node name
            "SubTextBox", // scan node price background
            "Panel (2)", // pause menu background
            "Panel (1)", // pause menu outer decoration
            "PleaseConfirmChangesPanel/Panel"
        };

        private static List<string> _particleMaterialNames = new()
        {
            "SparkParticle", // zapgun sparks
            "BeamUpZap", // teleporter
            "BeamUpZap 1" // inverse teleporter
        };

        private List<MaterialReplacement> _materialReplacements = new();

        private List<ImageShaderReplacement> _imageShaderReplacements = new();

        // works differently as-is because it's only changing PlayOneShot and nothing is being permanently patched
        private List<SoundReplacementBehaviour> _soundReplacements = new();

        public void CreateSoundReplacements(List<SoundReplacementBehaviour> soundReplacements)
        {
            _soundReplacements = soundReplacements;
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

            _materialReplacements.Add(new MaterialNameShaderReplacement(Plugin.RainbowDropshipLightsShader, "LEDLightYellow", false)); // item dropship lights
            _materialReplacements.Add(new ColorReplacement(new Color(0.9f, 0, 0, 1), "LEDLightYellow", "_Color")); // make item dropship lights more vibrant
            _materialReplacements.Add(new ColorReplacement(new Color(0, 0, 0.2f, 0.3f), "GlassTex", "_BaseColor")); // make magnifying glass more clear
            _materialReplacements.Add(new FloatReplacement(0.85f, "GlassTex", "_Smoothness")); // make magnifying glass less blurry

            _materialReplacements.Add(new MaterialNameShaderReplacement(Plugin.RainbowUIShader, "TankLight", false)); // apparatus light
            _materialReplacements.Add(new ColorReplacement(new Color(0.8f, 0, 0, 1), "TankLight", "_Color")); // apparatus light brightness
            _materialReplacements.Add(new FloatReplacement(5f, "TankLight", "_HueShiftSpeed")); // apparatus light speed

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
        }

        public void ReplaceSoundIfNeeded(ref AudioClip clip)
        {
            if (clip == null) return;
            string clipName = clip.name;
            var newClip = _soundReplacements.FirstOrDefault(x => x.SoundName == clipName);

            if (newClip == null) return;
            clip = newClip.ReplacementClip;
        }
    }
}
