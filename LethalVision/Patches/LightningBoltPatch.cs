using DigitalRuby.ThunderAndLightning;
using GameNetcodeStuff;
using HarmonyLib;
using LethalVision.Visuals;
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

        [HarmonyPatch(typeof(LightningBoltScript))]
        [HarmonyPatch("UpdateShaderParameters")]
        [HarmonyPostfix]
        private static void CreateDependencies(LightningBoltScript __instance)
        {
            if (!LethalVisuals.LethalVisualsEnabled) return;
            if (!Config.Instance.RainbowZapGun.Value) return;
            // HSV instead of OKHSL :-(
            // I don't wanna reimplement it in C# instead of shadercode rightnow as the implementation is fairly involved (probably 300-500 lines)
            // I wanted to initially use shadercode here but I don't have the funds to buy the lightning asset - it uses a special shader I'd have to modify.
            // TODO: either convert OKHSL to C# or have a compute shader/shader that can easily transfer to c#
            Color.RGBToHSV(new Color(1, 0, 0), out float H, out float S, out float V);
            H = (H + (Time.unscaledTime / 5)) % 1; // should maybe use deltatime/persistence instead? idk
            __instance.lightningMaterialMeshInternal.SetColor(_tintColorProperty, Color.HSVToRGB(H, S, V));
            __instance.lightningMaterialMeshNoGlowInternal.SetColor(_tintColorProperty, Color.HSVToRGB(H, S, V));
        }
    }
}
