using HarmonyLib;
using LethalVision.Visuals;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Patches
{
    [HarmonyPatch(typeof(JesterAI))]
    internal class JesterPatches
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPrefix(JesterAI __instance)
        {
            if (!LethalVisuals.LethalVisualsEnabled) return;
            if (Plugin.JesterHat == null) return;
            if (!Config.Instance.JesterHat.Value) return;

            var hatParent = __instance.transform.Find("MeshContainer/AnimContainer/metarig/BoxContainer/spine.004/spine.005/spine.006/UpperJaw");
            if (hatParent == null) return;

            var hat = UnityEngine.Object.Instantiate(Plugin.JesterHat, hatParent, false);
            hat.transform.localPosition = Vector3.zero;
            hat.transform.localRotation = Quaternion.identity;
            hat.transform.localScale = Vector3.one;
        }
    }
}
