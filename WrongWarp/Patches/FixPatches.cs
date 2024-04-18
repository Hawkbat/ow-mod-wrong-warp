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

        [HarmonyPrefix, HarmonyPatch(typeof(AlignToSurfaceFluidDetector), nameof(AlignToSurfaceFluidDetector.OnVolumeActivated))]
        public static void AlignToSurfaceFluidDetector_OnVolumeActivated(AlignToSurfaceFluidDetector __instance, PriorityVolume volume)
        {
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem) return;
            if (__instance is not RaftFluidDetector) return;
            if (volume is FluidVolume fluidVolume)
            {
                __instance._alignmentFluid = fluidVolume;
            }
        }
    }
}
