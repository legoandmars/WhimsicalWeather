using LethalVision.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Models
{
    internal class TextureReplacement : MaterialReplacement
    {
        private Dictionary<Material, Texture?> _originalTexturesByMaterial = new();
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
            if (material == null || material.shader == null) return false;
            if (!material.name.StartsWith(_materialName)) return false;
            if (!material.HasProperty(_propertyName)) return false;

            return true;
        }

        public override void ReplaceOnMaterial(Material material)
        {
            if (!material.HasProperty(_propertyName))
            {
                Debug.LogWarning($"Attempted to replace texture on {material.name} but property {_propertyName} does not exist!");
                return;
            }
            if (_replacedMaterials.Contains(material)) return;
            
            _replacedMaterials.Add(material);
            _originalTexturesByMaterial.Add(material, material.GetTexture(_propertyName));

            material.SetTexture(_propertyName, _replacement);
        }

        public override void RestoreMaterials()
        {
            foreach (var material in _replacedMaterials)
            {
                if (material == null) continue;
                material.SetTexture(_propertyName, _originalTexturesByMaterial[material]); // texture set could be null, which will be blank
            }

            _replacedMaterials.Clear();
            _originalTexturesByMaterial.Clear();
        }

        public override string ToString()
        {
            return $"MaterialReplacement (Texture): {_replacement.name} (replacing {_propertyName} on {_materialName})";
        }
    }
}
