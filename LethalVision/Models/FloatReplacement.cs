using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Models
{
    internal class FloatReplacement : MaterialReplacement
    {
        private Dictionary<Material, float> _originalFloatsByMaterial = new();
        private float _replacement;
        private string _materialName;
        private string _floatName;

        public FloatReplacement(float replacement, string materialName, string floatName)
        {
            _replacement = replacement;
            _materialName = materialName;
            _floatName = floatName;
        }

        public override bool MaterialIsMatch(Material material)
        {
            if (material == null || material.shader == null) return false;
            if (!material.name.StartsWith(_materialName)) return false;
            if (!material.HasFloat(_floatName)) return false;

            return true;
        }

        public override void ReplaceOnMaterial(Material material)
        {
            if (!material.HasFloat(_floatName))
            {
                Plugin.Logger.LogWarning($"Attempted to replace float on {material.name} but float {_floatName} does not exist!");
                return;
            }
            if (_replacedMaterials.Contains(material)) return;

            _replacedMaterials.Add(material);
            _originalFloatsByMaterial.Add(material, material.GetFloat(_floatName));

            material.SetFloat(_floatName, _replacement);
        }

        public override void RestoreMaterials()
        {
            foreach (var material in _replacedMaterials)
            {
                if (material == null) continue;
                material.SetFloat(_floatName, _originalFloatsByMaterial[material]); // texture set could be null, which will be blank
            }

            _replacedMaterials.Clear();
            _originalFloatsByMaterial.Clear();
        }

        public override string ToString()
        {
            return $"MaterialReplacement (Float): {_replacement} (replacing {_floatName} on {_materialName})";
        }
    }
}
