using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Patches
{
    [HarmonyPatch]
    public static class SignalPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlayerData), nameof(PlayerData.KnowsSignal))]
        public static bool PlayerData_KnowsSignal(SignalName __0, ref bool __result)
        {
            if (WrongWarpMod.Instance.DeviceSignals.IsManagedSignal(__0))
            {
                __result = WrongWarpMod.Instance.DeviceSignals.IsSignalKnown(__0);
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerData), nameof(PlayerData.LearnSignal))]
        public static bool PlayerData_LearnSignal(SignalName __0)
        {
            if (WrongWarpMod.Instance.DeviceSignals.IsManagedSignal(__0))
            {
                WrongWarpMod.Instance.DeviceSignals.LearnSignal(__0);
                return false;
            }
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AudioSignal), nameof(AudioSignal.SignalNameToString))]
        public static void AudioSignal_SignalNameToString(ref string __result)
        {
            __result = WrongWarpMod.Instance.DeviceSignals.GetCustomDisplayName(__result);
        }
    }
}
