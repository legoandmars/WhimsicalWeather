using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Models
{
    internal class MaterialNameShaderReplacement : MaterialReplacement
    {
        private Dictionary<Material, Shader?> _originalShadersByMaterial = new();
        private Shader _replacement;
        private string _materialName;
        private bool _isParticle;

        public MaterialNameShaderReplacement(Shader replacement, string materialName, bool isParticle)
        {
            _materialName = materialName;
            _replacement = replacement;
            _isParticle = isParticle;
        }

        public override bool MaterialIsMatch(Material material)
        {
            if (material == null || material.shader == null) return false;
            return material.name == _materialName;
        }

        public override void ReplaceOnMaterial(Material material)
        {
            if (_replacedMaterials.Contains(material)) return;

            _replacedMaterials.Add(material);
            _originalShadersByMaterial.Add(material, material.shader);

            // texture property name swap for some unlit particles
            if (_isParticle)
            {
                if (material.shader.name == "HDRP/Unlit")
                {
                    var texture = material.GetTexture("_UnlitColorMap");
                    material.shader = _replacement;
                    material.SetTexture("_BaseColorMap", texture);
                }
                else
                {
                    material.shader = _replacement;
                }
            }
            else
            {
                material.shader = _replacement;
            }
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
            return $"MaterialReplacement (Material Name): {_replacement.name} (replacing {_materialName})";
        }
    }
}
