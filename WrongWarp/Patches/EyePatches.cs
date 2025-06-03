using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Patches
{
    [HarmonyPatch]
    public static class EyePatches
    {
        /*
        [HarmonyPrefix, HarmonyPatch(typeof(QuantumCampsiteController), nameof(QuantumCampsiteController.GetTravelerMusicEndClip))]
        public static bool QuantumCampsiteController_GetTravelerMusicEndClip(QuantumCampsiteController __instance, ref AudioClip __result)
        {
            if (WrongWarpMod.Instance.EyeSequence.IsApostateEnding())
            {
                var hasSolanum = __instance._hasMetSolanum;
                var hasPrisoner = __instance._hasMetPrisoner && !__instance._hasErasedPrisoner;
                __result = WrongWarpMod.Instance.EyeSequence.GetEndingClip(hasSolanum, hasPrisoner);
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(TravelerEyeController), nameof(TravelerEyeController.OnCrossfadeToFinale))]
        public static void TravelerEyeController_OnCrossfadeToFinale(TravelerEyeController __instance, float __0)
        {
            WrongWarpMod.Instance.EyeSequence.StartFinale();
        }
        */

        [HarmonyPrefix, HarmonyPatch(typeof(NomaiCoordinateInterface), nameof(NomaiCoordinateInterface.SetPillarRaised), typeof(bool))]
        public static bool NomaiCoordinateInterface_SetPillarRaised(NomaiCoordinateInterface __instance, bool __0)
        {
            if (LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse)
            {
                if (__0 && !__instance._powered)
                {
                    return false;
                }
                if (__0 != __instance._pillarRaised)
                {
                    __instance._pillarRaised = __0;
                    __instance._upperOrb.RemoveAllLocks();
                    __instance._orb.RemoveAllLocks();
                    __instance._upperOrb.AddLock();
                    __instance._orb.AddLock();
                    __instance._updateHeight = true;
                    __instance._targetHeight = (__0 ? 0f : __instance._loweredHeight);
                    __instance._loopingSource.FadeIn(1f, false, false, 1f);
                }
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(VesselWarpController), nameof(VesselWarpController.Start))]
        public static void VesselWarpController_Start(VesselWarpController __instance)
        {
            __instance._sourceWarpPlatform.OnReceiveWarpedBody += _sourceWarpPlatform_OnReceiveWarpedBody;
            __instance._targetWarpPlatform.OnReceiveWarpedBody += _targetWarpPlatform_OnReceiveWarpedBody;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(VesselWarpController), nameof(VesselWarpController.OnDestroy))]
        public static void VesselWarpController_OnDestroy(VesselWarpController __instance)
        {
            __instance._sourceWarpPlatform.OnReceiveWarpedBody -= _targetWarpPlatform_OnReceiveWarpedBody;
            __instance._targetWarpPlatform.OnReceiveWarpedBody -= _targetWarpPlatform_OnReceiveWarpedBody;
        }

        private static void _sourceWarpPlatform_OnReceiveWarpedBody(OWRigidbody warpedBody, NomaiWarpPlatform startPlatform, NomaiWarpPlatform targetPlatform)
        {
            if (warpedBody.CompareTag("Player"))
            {
                ResetWarpPlatform(warpedBody, startPlatform, targetPlatform);
                var volume = targetPlatform.GetComponentInParent<VesselWarpController>()._bridgeVolume;
                volume.AddObjectToVolume(Locator.GetPlayerDetector().gameObject);
                volume.AddObjectToVolume(Locator.GetPlayerCamera().GetComponentInChildren<FluidDetector>().gameObject);
            } else
            {
                ResetWarpPlatform(warpedBody, targetPlatform, startPlatform);
            }
        }

        private static void _targetWarpPlatform_OnReceiveWarpedBody(OWRigidbody warpedBody, NomaiWarpPlatform startPlatform, NomaiWarpPlatform targetPlatform)
        {
            if (warpedBody.CompareTag("Player"))
            {
                ResetWarpPlatform(warpedBody, startPlatform, targetPlatform);
            } else
            {
                ResetWarpPlatform(warpedBody, targetPlatform, startPlatform);
            }
        }

        private static void ResetWarpPlatform(OWRigidbody warpedBody, NomaiWarpPlatform startPlatform, NomaiWarpPlatform targetPlatform)
        {
            UnityUtils.DoAfterSeconds(WrongWarpMod.Instance, 2f, () =>
            {
                UnityUtils.DoWhen(WrongWarpMod.Instance, () =>
                    (!warpedBody || !warpedBody.CompareTag("Player") || Vector3.Distance(targetPlatform.transform.position, warpedBody.transform.position) > 10f) &&
                    !targetPlatform.IsPlayerOnPlatform() &&
                    !targetPlatform.IsProbeOnPlatform() &&
                    !targetPlatform.IsBlackHoleOpen(), () =>
                    {
                        targetPlatform.OpenBlackHole(startPlatform, true);
                    });
            });
            
        }
    }
}
