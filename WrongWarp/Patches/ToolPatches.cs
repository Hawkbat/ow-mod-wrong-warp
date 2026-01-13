using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Components;

namespace WrongWarp.Patches
{
    [HarmonyPatch]
    public static class ToolPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.EquipToolMode))]
        public static bool ToolModeSwapper_EquipToolMode(ToolModeSwapper __instance, ToolMode __0)
        {
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem)
            {
                return true;
            }
            if (__instance._currentToolGroup == ToolGroup.Suit && __instance._itemCarryTool.GetHeldItemType() == AmoebaGunItem.ItemType)
            {
                if (__0 == ToolMode.Probe && !OWInput.IsPressed(InputLibrary.cancel))
                {
                    // Prevent switching away from item mode to the probe if holding the amoeba gun and not pressing cancel
                    return false;
                }
            }
            return true;
        }

    }
}
