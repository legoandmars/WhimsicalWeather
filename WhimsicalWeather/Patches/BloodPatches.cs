using GameNetcodeStuff;
using HarmonyLib;
using WhimsicalWeather.Visuals;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhimsicalWeather.Patches
{
    // TODO: Replace with confetti or something
    [HarmonyPatch]
    internal class BloodPatches
    {
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DropBlood))]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AddBloodToBody))]
        [HarmonyPrefix]
        private static bool DropBloodPrefix()
        {
            if (!Config.Instance.RemovePlayerBlood.Value) return true;
            return !WhimsicalVisuals.WhimsicalVisualsEnabled;
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayerFromOtherClientClientRpc))]
        [HarmonyPostfix]
        private static void DamagePlayerFromOtherClientPostfix(PlayerControllerB __instance)
        {
            if (!Config.Instance.RemovePlayerBlood.Value) return;
            if (__instance.health < 6 && __instance.bodyBloodDecals[0] != null && __instance.bodyBloodDecals[0].activeSelf)
            {
                __instance.bodyBloodDecals[0].SetActive(false);
            }
        }

        [HarmonyPatch(typeof(DeadBodyInfo), nameof(DeadBodyInfo.MakeCorpseBloody))]
        [HarmonyPrefix]
        private static bool MakeCorpseBloodyPrefix()
        {
            if (!Config.Instance.RemovePlayerBlood.Value) return true;
            return !WhimsicalVisuals.WhimsicalVisualsEnabled;
        }
    }
}
