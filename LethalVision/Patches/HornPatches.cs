using HarmonyLib;
using LethalVision.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace LethalVision.Patches
{
    [HarmonyPatch(typeof(ShipAlarmCord))]
    internal class HornPatches
    {
        private static MethodInfo _modifiedHornPitchMethod = AccessTools.Method(typeof(HornPatches), "ModifiedHornPitch");
        private static MethodInfo _modifiedHornLowPitchMethod = AccessTools.Method(typeof(HornPatches), "ModifiedHornLowPitch");

        [HarmonyPatch(typeof(ShipAlarmCord), "Update")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ShipHornUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].operand is 0.97f)
                {
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = _modifiedHornPitchMethod;
                }
                else if(codes[i].opcode == OpCodes.Ldc_R4 && codes[i].operand is 0.88f)
                {
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = _modifiedHornLowPitchMethod;
                }
            }
            return codes.Where(x => x.opcode != OpCodes.Nop).AsEnumerable();
        }

        public static float ModifiedHornPitch()
        {
            return LethalVisuals.LethalVisualsEnabled && Config.Instance.ChangeShipHornPitch.Value ? Config.Instance.ShipHornPitch.Value : 0.97f;
        }

        public static float ModifiedHornLowPitch()
        {
            return LethalVisuals.LethalVisualsEnabled && Config.Instance.ChangeShipHornPitch.Value ? Config.Instance.ShipHornStartPitch.Value : 0.88f;
        }
    }
}
