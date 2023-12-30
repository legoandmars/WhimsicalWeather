using LethalVision.Behaviours;
using LethalVision.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Controllers
{
    internal class ReplacementController
    {
        private static List<string> _imageSpriteNames = new()
        {
            "DialogueBox1Frame 2", // used for chat menu, inventory slots, etc
            "SprintMeter",
            "scanCircle1",
            "scanCircle2",
            "ScanUI1",
            "DialogueBoxSimple",
            "BatteryIcon",
            "BatteryMeter",
            "BatteryMeterFrame"
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

        private List<MaterialReplacement> _materialReplacements = new();
        private List<ImageShaderReplacement> _imageShaderReplacements = new();

        public void CreateTextureReplacements(List<TextureReplacementBehaviour> textureReplacements)
        {
            foreach (var replacement in textureReplacements)
            {
                _materialReplacements.Add(new TextureReplacement(replacement));
            }
        }

        public void CreateImageShaderReplacements()
        {
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
        }

        private void ReplaceTextures(List<Material> materials)
        {
            foreach (var replacementInfo in _materialReplacements)
            {
                foreach (var material in materials)
                {
                    replacementInfo.ReplaceMaterialIfMatch(material);
                }

                if (!replacementInfo.MaterialsReplaced)
                {
                    Debug.LogWarning($"No valid materials found for material replacement {replacementInfo}");
                }
            }
        }
    }
}
