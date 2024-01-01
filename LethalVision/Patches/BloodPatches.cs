using GameNetcodeStuff;
using HarmonyLib;
using LethalVision.Visuals;
using System;
using System.Collections.Generic;
using System.Text;

namespace LethalVision.Patches
{
    // TODO: Replace with confetti or something
    [HarmonyPatch]
    internal class BloodPatches
    {
        [HarmonyPatch(typeof(PlayerControllerB))]
        [HarmonyPatch(nameof(PlayerControllerB.DropBlood))]
        [HarmonyPatch(nameof(PlayerControllerB.AddBloodToBody))]
        [HarmonyPrefix]
        private static bool DropBloodPrefix(PlayerControllerB __instance)
        {
            return !LethalVisuals.LethalVisualsEnabled;
        }

        [HarmonyPatch(typeof(PlayerControllerB))]
        [HarmonyPatch(nameof(PlayerControllerB.DamagePlayerFromOtherClientClientRpc))]
        [HarmonyPostfix]
        private static void DamagePlayerFromOtherClientPostfix(PlayerControllerB __instance)
        {
            if (__instance.health < 6 && __instance.bodyBloodDecals[0] != null && __instance.bodyBloodDecals[0].activeSelf)
            {
                __instance.bodyBloodDecals[0].SetActive(false);
            }
        }

        [HarmonyPatch(typeof(DeadBodyInfo))]
        [HarmonyPatch(nameof(DeadBodyInfo.MakeCorpseBloody))]
        [HarmonyPrefix]
        private static bool MakeCorpseBloodyPrefix()
        {
            return !LethalVisuals.LethalVisualsEnabled;
        }
    }
}
