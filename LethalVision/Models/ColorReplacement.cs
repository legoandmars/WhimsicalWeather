using LethalVision.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Models
{
    internal class ColorReplacement : MaterialReplacement
    {
        private Dictionary<Material, Color> _originalColorsByMaterial = new();
        private Color _replacement;
        private string _materialName;
        private string _colorName;

        public ColorReplacement(Color replacement, string materialName, string colorName)
        {
            _replacement = replacement;
            _materialName = materialName;
            _colorName = colorName;
        }

        public override bool MaterialIsMatch(Material material)
        {
            if (material == null || material.shader == null) return false;
            if (!material.name.StartsWith(_materialName)) return false;
            if (!material.HasColor(_colorName)) return false;

            return true;
        }

        public override void ReplaceOnMaterial(Material material)
        {
            if (!material.HasColor(_colorName))
            {
                Debug.LogWarning($"Attempted to replace color on {material.name} but color {_colorName} does not exist!");
                return;
            }
            if (_replacedMaterials.Contains(material)) return;

            _replacedMaterials.Add(material);
            _originalColorsByMaterial.Add(material, material.GetColor(_colorName));

            material.SetColor(_colorName, _replacement);
        }

        public override void RestoreMaterials()
        {
            foreach (var material in _replacedMaterials)
            {
                if (material == null) continue;
                material.SetColor(_colorName, _originalColorsByMaterial[material]); // texture set could be null, which will be blank
            }

            _replacedMaterials.Clear();
            _originalColorsByMaterial.Clear();
        }

        public override string ToString()
        {
            return $"MaterialReplacement (Color): {_replacement} (replacing {_colorName} on {_materialName})";
        }
    }
}
