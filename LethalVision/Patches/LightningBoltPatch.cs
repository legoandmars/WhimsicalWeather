using DigitalRuby.ThunderAndLightning;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.ParticleSystem;

namespace LethalVision.Patches
{
    [HarmonyPatch]
    internal class LightningBoltPatch
    {
        private static int _tintColorProperty = Shader.PropertyToID("_TintColor");
        private static MinMaxGradient _rainbowGradient;

        // kinda fucked, but we need it for random particle colors lol
        static LightningBoltPatch()
        {
            var gradientColors = new List<GradientColorKey>();
            // 10 steps is *probably* enough?
            int steps = 10;
            for (int i = 0; i < steps; i++)
            {
                gradientColors.Add(GetHsvColorAtGradientPoint((1.0f / steps) * i));
            }

            var alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0].alpha = 1.0F;
            alphaKeys[0].time = 0.0F;
            alphaKeys[1].alpha = 0.0F;
            alphaKeys[1].time = 1.0F;

            var gradient = new Gradient();
            gradient.SetKeys(gradientColors.ToArray(), alphaKeys);
            _rainbowGradient = new MinMaxGradient(gradient);
        }

        private static GradientColorKey GetHsvColorAtGradientPoint(float point)
        {
            Color.RGBToHSV(new Color(1, 0, 0), out float H, out float S, out float V);
            H = (H + point) % 1;
            var color = Color.HSVToRGB(H, S, V);

            return new GradientColorKey(color, point);
        }

        [HarmonyPatch(typeof(LightningBoltScript))]
        [HarmonyPatch("UpdateShaderParameters")]
        [HarmonyPostfix]
        private static void CreateDependencies(LightningBoltScript __instance)
        {
            Debug.Log("Updating lightning deps!");
            // HSV instead of OKHSL :-(
            // I don't wanna reimplement it in C# instead of shadercode rightnow as the implementation is fairly involved (probably 300-500 lines)
            // I wanted to initially use shadercode here but I don't have the funds to buy the lightning asset - it uses a special shader I'd have to modify.
            Color.RGBToHSV(new Color(1, 0, 0), out float H, out float S, out float V);
            H = (H + (Time.unscaledTime / 5)) % 1;
            __instance.lightningMaterialMeshInternal.SetColor(_tintColorProperty, Color.HSVToRGB(H, S, V));
            __instance.lightningMaterialMeshNoGlowInternal.SetColor(_tintColorProperty, Color.HSVToRGB(H, S, V));
            //Debug.Log(__result.LightningMaterialMesh.color);
            //Debug.Log(__result.LightningMaterialMeshNoGlow.color);
            //__result.LightningMaterialMesh.color = new Color(1, 0, 0, 1);
            //__result.LightningMaterialMeshNoGlow.color = new Color(1, 0, 0, 1);
            //__result.LightningMaterialMesh.shader = Plugin.RainbowLerpShader;
            //__result.LightningMaterialMeshNoGlow.shader = Plugin.RainbowLerpShader;
        }

        [HarmonyPatch(typeof(PatcherTool))]
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void PatcherTool(PatcherTool __instance) // i hardly know 'er tool!
        {
            // TODO: impl check based on if we're currently in fairy mode
            Debug.Log("Updating patcher tool!");
            var particleTransform = __instance.transform.Find("Effects/SparkParticle (1)");
            if (particleTransform == null) return;

            var particles = particleTransform.GetComponent<ParticleSystem>();
            var main = particles.main;
            //main.startColor = _rainbowGradient;

            var renderer = particleTransform.GetComponent<ParticleSystemRenderer>();
            renderer.material.shader = Plugin.RainbowParticleShader;
            Debug.Log("AAA===");
            //Debug.Log(__result.LightningMaterialMesh.color);
            //Debug.Log(__result.LightningMaterialMeshNoGlow.color);
            //__result.LightningMaterialMesh.color = new Color(1, 0, 0, 1);
            //__result.LightningMaterialMeshNoGlow.color = new Color(1, 0, 0, 1);
            //__result.LightningMaterialMesh.shader = Plugin.RainbowLerpShader;
            //__result.LightningMaterialMeshNoGlow.shader = Plugin.RainbowLerpShader;
        }
    }
}
