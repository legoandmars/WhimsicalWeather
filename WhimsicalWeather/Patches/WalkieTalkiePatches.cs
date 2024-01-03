using GameNetcodeStuff;
using HarmonyLib;
using WhimsicalWeather.Visuals;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace WhimsicalWeather.Patches
{
    [HarmonyPatch(typeof(WalkieTalkie))]
    internal class WalkieTalkiePatches
    {
        public const float WalkieTalkieLightIntensityOverride = 0.1f;
        public const float WalkieTalkieLightIntensityDefault = 0.4843846f;

        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        private static void StartPrefix(PlayerControllerB __instance)
        {
            if (!WhimsicalVisuals.WhimsicalVisualsEnabled) return;
            SetWalkieLightIntensity(WalkieTalkieLightIntensityOverride);
        }

        public static void SetWalkieLightIntensity(float intensity)
        {
            foreach (var walkie in WalkieTalkie.allWalkieTalkies)
            {
                if (walkie == null || walkie.walkieTalkieLight == null) continue;
                walkie.walkieTalkieLight.intensity = intensity;
            }
        }
    }
}
