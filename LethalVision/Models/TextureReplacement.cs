using LethalVision.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Models
{
    // used to keep track of changes we make to the game so we can undo them
    internal class TextureReplacement : MaterialReplacement
    {
        private Texture? _original; // assumes the original will be the same for all materials. always the case now but might need changing

        private Texture _replacement;
        private string _materialName;
        private string _propertyName;

        public TextureReplacement(TextureReplacementBehaviour textureReplacement)
        {
            _replacement = textureReplacement.Texture;
            _materialName = textureReplacement.MaterialName;
            _propertyName = textureReplacement.PropertyName;
        }

        public override bool MaterialIsMatch(Material material)
        {
            if (!material.name.StartsWith(_materialName)) return false;
            if (!material.HasProperty(_propertyName)) return false;

            return true;
        }

        public override void ReplaceMaterialIfMatch(Material material)
        {
            if (!MaterialIsMatch(material)) return;
            ReplaceOnMaterial(material);
        }

        public override void ReplaceOnMaterial(Material material)
        {
            if (!material.HasProperty(_propertyName))
            {
                Debug.LogWarning($"Attempted to replace texture on {material.name} but property {_propertyName} does not exist!");
                return;
            }

            if (_original == null)
            {
                _original = material.GetTexture(_propertyName);
            }
            material.SetTexture(_propertyName, _replacement);
            _replacedMaterials.Add(material);
        }

        public override void RestoreMaterials()
        {
            foreach (var material in _replacedMaterials)
            {
                material.SetTexture(_propertyName, _original); // texture set could be null, which will be blank
            }

            _replacedMaterials.Clear();
        }

        public override string ToString()
        {
            return $"MaterialReplacement (Texture): {_replacement.name}";
        }
    }
}
