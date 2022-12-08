using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OWML.Common;
using UnityEngine;

namespace WrongWarp
{
    public static class Patches
    {
        public static WrongWarpMod Mod;

        public static void Initialize(WrongWarpMod mod)
        {
            Mod = mod;

            ApplyPatches();
        }

        public static void ApplyPatches()
        {
            //Mod.ModHelper.HarmonyHelper.AddPrefix<NomaiCoordinateInterface>(nameof(NomaiCoordinateInterface.CheckEyeCoordinates), typeof(Patches), nameof(NomaiCoordinateInterface_CheckEyeCoordinates));
            //Mod.ModHelper.HarmonyHelper.AddPrefix<VesselWarpController>(nameof(VesselWarpController.OnSlotActivated), typeof(Patches), nameof(VesselWarpController_OnSlotActivated));
            //Mod.ModHelper.HarmonyHelper.AddPrefix<VesselWarpController>(nameof(VesselWarpController.WarpVessel), typeof(Patches), nameof(VesselWarpController_WarpVessel));
            Mod.ModHelper.HarmonyHelper.AddPrefix<TestTagScript>(nameof(TestTagScript.Awake), typeof(Patches), nameof(TestTagScript_Awake));
            Mod.ModHelper.HarmonyHelper.AddPrefix(typeof(PlayerData).GetMethod(nameof(PlayerData.KnowsSignal)), typeof(Patches), nameof(PlayerData_KnowsSignal));
            Mod.ModHelper.HarmonyHelper.AddPrefix(typeof(PlayerData).GetMethod(nameof(PlayerData.LearnSignal)), typeof(Patches), nameof(PlayerData_LearnSignal));
            Mod.ModHelper.HarmonyHelper.AddPrefix<NotificationDisplay.NotificationDisplayData>(nameof(NotificationDisplay.NotificationDisplayData.IncrementTimeDisplayed), typeof(Patches), nameof(NotificationDisplayData_IncrementTimeDisplayed));
            Mod.ModHelper.HarmonyHelper.AddPostfix<AudioSignal>(nameof(AudioSignal.SignalNameToString), typeof(Patches), nameof(AudioSignal_SignalNameToString));
            Mod.ModHelper.HarmonyHelper.AddPrefix<QuantumCampsiteController>(nameof(QuantumCampsiteController.GetTravelerMusicEndClip), typeof(Patches), nameof(QuantumCampsiteController_GetTravelerMusicEndClip));
            Mod.ModHelper.HarmonyHelper.AddPrefix<TravelerEyeController>(nameof(TravelerEyeController.OnCrossfadeToFinale), typeof(Patches), nameof(TravelerEyeController_OnCrossfadeToFinale));
            Mod.ModHelper.HarmonyHelper.AddPrefix<TabbedMenu>(nameof(TabbedMenu.OnUpdateInputDevice), typeof(Patches), nameof(TabbedMenu_OnUpdateInputDevice));
        }
        /*
        public static bool NomaiCoordinateInterface_CheckEyeCoordinates(NomaiCoordinateInterface __instance, ref bool __result)
        {
            bool isWrongWarp = Mod.Warp.CheckWrongWarpCoordinates(__instance);
            if (isWrongWarp)
            {
                __result = true;
                return false;
            }
            return true;
        }

        public static bool VesselWarpController_OnSlotActivated(VesselWarpController __instance, NomaiInterfaceSlot slot)
        {
            var isWrongWarp = Mod.Warp.CheckWrongWarpCoordinates(__instance._coordinateInterface);
            if (isWrongWarp)
            {
                if (slot == __instance._warpVesselSlot && __instance._hasPower && __instance._blackHole.GetState() == SingularityController.State.Collapsed)
                {
                    __instance._blackHole.Create();
                    RumbleManager.StartVesselWarp();
                    __instance._openingBlackHole = true;
                    __instance.enabled = true;
                    __instance._blackHoleOneShot.PlayOneShot(AudioType.VesselSingularityCreate, 1f);
                    Mod.Warp.WarpToWrongWarpSystem(true);
                }
                return false;
            }
            return true;
        }

        public static bool VesselWarpController_WarpVessel(VesselWarpController __instance)
        {
            var isWrongWarp = Mod.Warp.CheckWrongWarpCoordinates(__instance._coordinateInterface);
            if (isWrongWarp)
            {
                return false;
            }
            return true;
        }
        */
        public static bool TestTagScript_Awake(TestTagScript __instance)
        {
            if (__instance.transform.parent && __instance.gameObject.name.StartsWith("$"))
            {
                Mod.ApplyModObjectType(__instance.transform.parent.gameObject, __instance.gameObject.name.Substring(1));
                UnityEngine.Object.Destroy(__instance.gameObject);
                return false;
            }
            return true;
        }

        public static bool PlayerData_KnowsSignal(SignalName __0, ref bool __result)
        {
            if (Mod.DeviceSignals.IsManagedSignal(__0))
            {
                __result = Mod.DeviceSignals.IsSignalKnown(__0);
                return false;
            }
            return true;
        }

        public static bool PlayerData_LearnSignal(SignalName __0)
        {
            if (Mod.DeviceSignals.IsManagedSignal(__0))
            {
                Mod.DeviceSignals.LearnSignal(__0);
                return false;
            }
            return true;
        }

        public static bool NotificationDisplayData_IncrementTimeDisplayed(NotificationDisplay.NotificationDisplayData __instance)
        {
            if (Mod.SignalTowers.IsActiveNotification(__instance.Data))
            {
                __instance.TextToDisplay = Mod.SignalTowers.GetNotificationText();
                __instance.TextDisplay.text = __instance.TextToDisplay;
                __instance.TextScrollEffect._strToDisplay = __instance.TextToDisplay;
                __instance.TextScrollEffect.SetTypingTime(TextSpeed.Instant);
                __instance.TextScrollEffect.CompleteTextEffect();
            }
            return true;
        }

        public static void AudioSignal_SignalNameToString(ref string __result)
        {
            __result = Mod.DeviceSignals.GetCustomDisplayName(__result);
        }
        
        public static bool QuantumCampsiteController_GetTravelerMusicEndClip(QuantumCampsiteController __instance, ref AudioClip __result)
        {
            if (Mod.EyeSequence.IsApostateEnding())
            {
                var hasSolanum = __instance._hasMetSolanum;
                var hasPrisoner = __instance._hasMetPrisoner && !__instance._hasErasedPrisoner;
                __result = Mod.EyeSequence.GetEndingClip(hasSolanum, hasPrisoner);
                return false;
            }
            return true;
        }

        public static bool TravelerEyeController_OnCrossfadeToFinale(TravelerEyeController __instance, float __0)
        {
            Mod.EyeSequence.StartFinale();
            return true;
        }

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
