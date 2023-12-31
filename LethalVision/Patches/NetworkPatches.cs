using HarmonyLib;
using LethalVision.Visuals;
using System;
using System.Collections.Generic;
using System.Text;

namespace LethalVision.Patches
{
    // prevent material changes from leaking over if the game quits
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class NetworkPatches
    {
        [HarmonyPatch(nameof(GameNetworkManager.Disconnect))]
        [HarmonyPrefix]
        private static void OnDisconnect(GameNetworkManager __instance)
        {
            if (__instance.isDisconnecting || StartOfRound.Instance == null || !LethalVisuals.LethalVisualsEnabled) return;
            if (LethalVisuals.LethalVisualsEnabled)
            {
                LethalVisuals.ToggleVisualsEvent(false);
            }

            Plugin.WeatherEvents.EventsEnabled = false;
            Plugin.WeatherEvents.gameObject.SetActive(false);
            Plugin.WeatherEvents.EventsEnabled = true;
        }
    }
}
