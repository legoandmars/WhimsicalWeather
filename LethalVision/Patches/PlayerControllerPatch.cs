using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

namespace LethalVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void StartPrefix(PlayerControllerB __instance)
        {
            Debug.Log("HDLethalCompany - Applying configs");

            UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(HDAdditionalCameraData));

            for (int i = 0; i < array.Length; i++)
            {
                HDAdditionalCameraData cameraData = array[i] as HDAdditionalCameraData;

                cameraData.customRenderingSettings = true;
                ToggleCustomPass(cameraData, true);
                
            }
            var additionalCameraData = __instance.gameplayCamera.GetComponent<HDAdditionalCameraData>();
            if (additionalCameraData != null)
            {
                Debug.Log("Setting additional camera data...");
                additionalCameraData.customRenderingSettings = true;
                ToggleCustomPass(additionalCameraData, true);
            }

            TextMeshProUGUI[] texts = Resources.FindObjectsOfTypeAll(typeof(TextMeshProUGUI)) as TextMeshProUGUI[];
            var materials = texts.Select(x => x.fontSharedMaterial).Distinct();
            foreach(var material in materials)
            {
                if (material.shader.name == "TextMeshPro/Distance Field")
                {
                    material.shader = Plugin.RainbowTextShader;
                    Debug.Log("Replacing.");
                }
            }
        }

        private List<string> _whiteListedSpritesRainbowUI = new()
        {
            "DialogueBox1Frame 2",
            "SprintMeter"
        };

        private void Rainbowify()
        {
            TextMeshProUGUI[] texts = Resources.FindObjectsOfTypeAll(typeof(TextMeshProUGUI)) as TextMeshProUGUI[];
            var materials = texts.Select(x => x.fontSharedMaterial).Distinct();
            foreach (var material in materials)
            {
                if (material.shader.name == "TextMeshPro/Distance Field")
                {
                    material.shader = Plugin.RainbowTextShader;
                    Debug.Log("Replacing.");
                }
            }

            Image[] images = Resources.FindObjectsOfTypeAll(typeof(Image)) as Image[];
            foreach (var image in images)
            {
                if(image.sprite?.name != null && _whiteListedSpritesRainbowUI.Any(x => x == image.sprite.name))
                {
                    // instance material
                    // TODO Fancier fix
                    image.material.shader = Plugin.RainbowUIShader;
                }
            }
            
        }

        public static void ToggleCustomPass(HDAdditionalCameraData cameraData, bool enable)
        {
            cameraData.renderingPathCustomFrameSettingsOverrideMask.mask[(int)FrameSettingsField.CustomPass] = true;
            cameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.CustomPass, enable);
        }
    }
}
