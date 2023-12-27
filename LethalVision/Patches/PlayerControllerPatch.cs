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

            Rainbowify();
        }

        private static List<string> _whiteListedSpritesRainbowUI = new()
        {
            "DialogueBox1Frame 2", // used for chat menu, inventory slots, etc
            "SprintMeter",
            "scanCircle1",
            "scanCircle2",
            "ScanUI1",
            "DialogueBoxSimple"
        };

        private static List<string> _whiteListedNamesRainbowUI = new()
        {
            "HeaderTextBox", // scan node name
            "SubTextBox", // scan node price background
            "Panel (2)", // pause menu background
            "Panel (1)" // pause menu outer decoration
        };

        private static void Rainbowify()
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
                // TODO: If there's no sprite, we can just use the vertex color to save a ton of performance
                // Come up with second vertex-only shader
                if((image.sprite?.name != null && _whiteListedSpritesRainbowUI.Any(x => x == image.sprite.name)) || _whiteListedNamesRainbowUI.Any(x => x == image.gameObject.name))
                {
                    // instance material
                    // TODO Fancier fix
                    image.material = UnityEngine.Object.Instantiate(image.material);
                    image.material.shader = Plugin.RainbowUIShader;
                }
                else if(image.gameObject.name == "Panel" && image.transform.parent.name == "PleaseConfirmChangesPanel")
                {
                    // specifically for the generically-named panel when confirming settings
                    image.material = UnityEngine.Object.Instantiate(image.material);
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
