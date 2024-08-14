using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Patches
{
    [HarmonyPatch]
    public static class FixPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(TabbedMenu), nameof(TabbedMenu.OnUpdateInputDevice))]
        public static bool TabbedMenu_OnUpdateInputDevice(TabbedMenu __instance)
        {
            if (__instance == null)
            {
                return false;
            }
            return true;
        }
    }
}
