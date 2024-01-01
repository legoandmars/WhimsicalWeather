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
        private static List<Shovel> _trackedShovels = new();
        public static bool _tracking = false;

        [HarmonyPatch(typeof(GrabbableObject))]
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPostfix(GrabbableObject __instance)
        {
            //   if (!LethalVisuals.LethalVisualsEnabled) return;
            if (!(__instance is Shovel shovel)) return;
            CreateShovelVisuals(shovel);
            _trackedShovels.Add(shovel);

            if (_tracking)
            {
                SetShovelVisuals(shovel, true);
            }
        }

        // fix stop/yield sign reappearing on grab
        [HarmonyPatch(typeof(GrabbableObject))]
        [HarmonyPatch(nameof(GrabbableObject.EnableItemMeshes))]
        [HarmonyPostfix]
        private static void EnableItemMeshesPostfix(GrabbableObject __instance, ref bool enable)
        {
            if (!enable) return;
            if (!(__instance is Shovel shovel)) return;
            if (!LethalVisuals.LethalVisualsEnabled) return;

            // used in stop sign/warn sign
            if (shovel.gameObject.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.enabled = false;
            }
        }

        // fix stop/yield sign reappearing on grab
        [HarmonyPatch(typeof(GrabbableObject))]
        [HarmonyPatch(nameof(GrabbableObject.GrabItemOnClient))]
        [HarmonyPatch(nameof(GrabbableObject.GrabItemFromEnemy))]
        [HarmonyPostfix]
        private static void EnableItemMeshesPostfix(GrabbableObject __instance)
        {
            if (!(__instance is Shovel shovel)) return;
            if (!LethalVisuals.LethalVisualsEnabled) return;

            // used in stop sign/warn sign
            if (shovel.gameObject.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.enabled = false;
            }
        }

        public static void ReplaceAllShovels()
        {
            foreach (var shovel in _trackedShovels)
            {
                if (shovel == null) continue;
                SetShovelVisuals(shovel, true);
            }
            _tracking = true;
        }

        public static void RestoreAllShovels()
        {
            foreach (var shovel in _trackedShovels)
            {
                if (shovel == null) continue;
                SetShovelVisuals(shovel, false);
            }
            _tracking = false;
        }

        private static void CreateShovelVisuals(Shovel shovel)
        {
            Transform visualsContainer = shovel.transform.Find("CustomVisuals");
            if (visualsContainer == null)
            {
                var visualsGameObject = new GameObject("CustomVisuals");
                visualsContainer = visualsGameObject.transform;
                visualsContainer.SetParent(shovel.transform);
                visualsContainer.transform.localPosition = Vector3.zero;
                visualsContainer.transform.localRotation = Quaternion.identity;
                visualsContainer.transform.localScale = Vector3.one;
            }
            foreach (Transform visual in visualsContainer)
            {
                visual.gameObject.SetActive(false);
            }

            var visuals = GetShovelVisuals(shovel);
            if (visuals == null) return;

            var lollypop = UnityEngine.Object.Instantiate(visuals, visualsContainer.transform, false);
            lollypop.transform.localPosition = Vector3.zero;
            lollypop.transform.localRotation = Quaternion.identity;
            lollypop.transform.localScale = Vector3.one;

            visualsContainer.gameObject.SetActive(false);
        }

        private static void SetShovelVisuals(Shovel shovel, bool enabled)
        {
            Transform visualsContainer = shovel.transform.Find("CustomVisuals");
            if (visualsContainer == null) return;
            visualsContainer.gameObject.SetActive(enabled);

            // used in shovel
            var mesh = shovel.transform.Find("mesh");
            if (mesh != null)
            {
                mesh.gameObject.SetActive(!enabled);
            }

            // used in stop sign/warn sign
            if (shovel.gameObject.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.enabled = !enabled;
            }
        }

        private static GameObject? GetShovelVisuals(Shovel shovel)
        {
            if (shovel.gameObject.name.StartsWith("ShovelItem")) return Plugin.GrayLollypop;
            else if (shovel.gameObject.name.StartsWith("YieldSign")) return Plugin.YellowLollypop;
            else if (shovel.gameObject.name.StartsWith("StopSign")) return Plugin.RedLollypop;
            return Plugin.RedLollypop;
        }
    }
}
