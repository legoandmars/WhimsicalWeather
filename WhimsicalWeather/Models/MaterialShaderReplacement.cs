using WhimsicalWeather.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WhimsicalWeather.Models
{
    // does NOT handle instantiation/duplication
    // make sure to filter the text material list using .Distinct()!
    // may eventually want more strict filtering but we chill for now
    internal class MaterialShaderReplacement : MaterialReplacement
    {
        private Dictionary<Material, Shader?> _originalShadersByMaterial = new();
        private Shader _replacement;
        private string _shaderName;

        public MaterialShaderReplacement(Shader replacement, string shaderName)
        {
            _shaderName = shaderName;
            _replacement = replacement;
        }

        public override bool MaterialIsMatch(Material material)
        {
            if (material == null || material.shader == null) return false;
            return material.shader.name == _shaderName;
        }

        public override void ReplaceOnMaterial(Material material)
        {
            if (_replacedMaterials.Contains(material)) return;

            _replacedMaterials.Add(material);
            _originalShadersByMaterial.Add(material, material.shader);

            material.shader = _replacement;

            if (Config.Instance.RainbowUIHueShiftSpeed.Value == 1f) return;
            if (material.shader != Plugin.RainbowUIShader && material.shader != Plugin.RainbowTextShader) return;
            material.SetFloat("_HueShiftSpeed", Config.Instance.RainbowUIHueShiftSpeed.Value);
        }

        public override void RestoreMaterials()
        {
            foreach (var material in _replacedMaterials)
            {
                if (material == null) continue;
                material.shader = _originalShadersByMaterial[material];
            }

            _replacedMaterials.Clear();
            _originalShadersByMaterial.Clear();
        }

        public override string ToString()
        {
            return $"MaterialReplacement (Shader): {_replacement.name} (replacing {_shaderName})";
        }
    }
}
