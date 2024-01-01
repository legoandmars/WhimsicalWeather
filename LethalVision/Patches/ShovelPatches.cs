using HarmonyLib;
using LethalVision.Visuals;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Patches
{
    [HarmonyPatch]
    internal class ShovelPatches
    {
        [HarmonyPatch(typeof(GrabbableObject))]
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPrefix(GrabbableObject __instance)
        {
            //   if (!LethalVisuals.LethalVisualsEnabled) return;
            if (!(__instance is Shovel shovel)) return;
            if (Plugin.RedLollypop == null) return;

            var mesh = __instance.transform.Find("mesh");
            if (mesh == null) return;
            mesh.gameObject.SetActive(false);

            var lollypop = UnityEngine.Object.Instantiate(Plugin.RedLollypop, __instance.transform, false);
            lollypop.transform.localPosition = Vector3.zero;
            lollypop.transform.localRotation = Quaternion.identity;
            lollypop.transform.localScale = Vector3.one;
        }
    }
}
