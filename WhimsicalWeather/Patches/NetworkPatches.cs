using HarmonyLib;
using WhimsicalWeather.Visuals;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhimsicalWeather.Patches
{
    // prevent material changes from leaking over if the game quits
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class NetworkPatches
    {
        [HarmonyPatch(nameof(GameNetworkManager.Disconnect))]
        [HarmonyPrefix]
        private static void OnDisconnect(GameNetworkManager __instance)
        {
            if (__instance.isDisconnecting || StartOfRound.Instance == null || !WhimsicalVisuals.WhimsicalVisualsEnabled) return;
            if (WhimsicalVisuals.WhimsicalVisualsEnabled)
            {
                WhimsicalVisuals.ToggleVisualsEvent(false);
            }

            Plugin.WeatherEvents.EventsEnabled = false;
            Plugin.WeatherEvents.gameObject.SetActive(false);
            Plugin.WeatherEvents.EventsEnabled = true;
        }
    }
}
