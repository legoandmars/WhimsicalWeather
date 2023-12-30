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
        private List<MaterialReplacement> materialReplacements = new();

        public void CreateTextureReplacements(List<TextureReplacementBehaviour> textureReplacements)
        {
            foreach (var replacement in textureReplacements)
            {
                materialReplacements.Add(new TextureReplacement(replacement));
            }
        }

        private void ReplaceTextures(List<Material> materials)
        {
            foreach (var replacementInfo in materialReplacements)
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
