using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OWML.Common;
using UnityEngine;
using WrongWarp.Utils;
using HarmonyLib;

namespace WrongWarp
{
    [HarmonyPatch]
    public static class Patches
    {
        public static WrongWarpMod Mod;

        public static void Initialize(WrongWarpMod mod)
        {
            Mod = mod;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        
        static GameObject shipLogDetectiveModeEntryCardTemplate;
        static GameObject shipLogDetectiveModeEntryLinkTemplate;
        static bool shipLogDetectiveModeMidInitialization;

        [HarmonyPrefix, HarmonyPatch(typeof(ShipLogDetectiveMode), nameof(ShipLogDetectiveMode.Initialize))]
        public static bool ShipLogDetectiveMode_Initialize_Prefix(ShipLogDetectiveMode __instance)
        {
            if (shipLogDetectiveModeMidInitialization) return false;
            shipLogDetectiveModeMidInitialization = true;
            if (__instance._cardList != null && __instance._cardList.Count > 0)
                shipLogDetectiveModeEntryCardTemplate = __instance._cardList[0].gameObject;
            else if (!shipLogDetectiveModeEntryCardTemplate && __instance._entryCardTemplate)
                shipLogDetectiveModeEntryCardTemplate = __instance._entryCardTemplate;
            if (__instance._linkList != null && __instance._linkList.Count > 0)
                shipLogDetectiveModeEntryLinkTemplate = __instance._linkList[0].gameObject;
            else if (!shipLogDetectiveModeEntryLinkTemplate && __instance._entryLinkTemplate)
                shipLogDetectiveModeEntryLinkTemplate = __instance._entryLinkTemplate;
            __instance._entryCardTemplate = GameObject.Instantiate(shipLogDetectiveModeEntryCardTemplate);
            __instance._entryLinkTemplate = GameObject.Instantiate(shipLogDetectiveModeEntryLinkTemplate);
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ShipLogDetectiveMode), nameof(ShipLogDetectiveMode.Initialize))]
        public static void ShipLogDetectiveMode_Initialize_Postfix(ShipLogDetectiveMode __instance)
        {
            __instance._entryCardTemplate = GameObject.Instantiate(shipLogDetectiveModeEntryCardTemplate);
            __instance._entryLinkTemplate = GameObject.Instantiate(shipLogDetectiveModeEntryLinkTemplate);
            shipLogDetectiveModeMidInitialization = false;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(CanvasGroupAnimator), nameof(CanvasGroupAnimator.SetImmediate), typeof(float), typeof(Vector3))]
        public static void CanvasGroupAnimator_SetImmediate(CanvasGroupAnimator __instance)
        {
            if (!__instance._canvasGroup)
            {
                __instance._canvasGroup = __instance.GetComponent<CanvasGroup>();
                __instance._rectTransform = __instance.GetComponent<RectTransform>();
                __instance._isComplete = true;
                __instance._updatingCanvases = false;
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(SingleInteractionVolume), nameof(SingleInteractionVolume.UpdatePromptVisibility))]
        public static void SingleInteractionVolume_UpdatePromptVisibility(SingleInteractionVolume __instance)
        {
            if (!__instance._playerCam) __instance._playerCam = Locator.GetPlayerCamera();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerAttachPoint), nameof(PlayerAttachPoint.InitAttachment))]
        public static void PlayerAttachPoint_InitAttachment(PlayerAttachPoint __instance)
        {
            if (!__instance._playerTransform)
            {
                __instance._playerTransform = Locator.GetPlayerTransform();
                __instance._playerOWRigidbody = __instance._playerTransform.GetRequiredComponent<OWRigidbody>();
                __instance._playerController = __instance._playerTransform.GetRequiredComponent<PlayerCharacterController>();
                __instance._fpsCamController = __instance._playerTransform.GetRequiredComponentInChildren<PlayerCameraController>();
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(TestTagScript), nameof(TestTagScript.Awake))]
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

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerData), nameof(PlayerData.KnowsSignal))]
        public static bool PlayerData_KnowsSignal(SignalName __0, ref bool __result)
        {
            if (Mod.DeviceSignals.IsManagedSignal(__0))
            {
                __result = Mod.DeviceSignals.IsSignalKnown(__0);
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerData), nameof(PlayerData.LearnSignal))]
        public static bool PlayerData_LearnSignal(SignalName __0)
        {
            if (Mod.DeviceSignals.IsManagedSignal(__0))
            {
                Mod.DeviceSignals.LearnSignal(__0);
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NotificationDisplay.NotificationDisplayData), nameof(NotificationDisplay.NotificationDisplayData.IncrementTimeDisplayed))]
        public static void NotificationDisplayData_IncrementTimeDisplayed(NotificationDisplay.NotificationDisplayData __instance)
        {
            if (Mod.SignalTowers.IsActiveNotification(__instance.Data))
            {
                __instance.TextToDisplay = Mod.SignalTowers.GetNotificationText();
                __instance.TextDisplay.text = __instance.TextToDisplay;
                __instance.TextScrollEffect._strToDisplay = __instance.TextToDisplay;
                __instance.TextScrollEffect.SetTypingTime(TextSpeed.Instant);
                __instance.TextScrollEffect.CompleteTextEffect();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AudioSignal), nameof(AudioSignal.SignalNameToString))]
        public static void AudioSignal_SignalNameToString(ref string __result)
        {
            __result = Mod.DeviceSignals.GetCustomDisplayName(__result);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(QuantumCampsiteController), nameof(QuantumCampsiteController.GetTravelerMusicEndClip))]
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

        [HarmonyPrefix, HarmonyPatch(typeof(TravelerEyeController), nameof(TravelerEyeController.OnCrossfadeToFinale))]
        public static void TravelerEyeController_OnCrossfadeToFinale(TravelerEyeController __instance, float __0)
        {
            Mod.EyeSequence.StartFinale();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(TabbedMenu), nameof(TabbedMenu.OnUpdateInputDevice))]
        public static bool TabbedMenu_OnUpdateInputDevice(TabbedMenu __instance)
        {
            if (__instance == null)
            {
                return false;
            }
            return true;
        }

        static bool isRespawn = false;

        [HarmonyPrefix, HarmonyPatch(typeof(DeathManager), nameof(DeathManager.KillPlayer))]
        public static bool DeathManager_KillPlayer(DeathManager __instance, DeathType __0)
        {
            if (!Mod.IsInWrongWarpSystem) return true;
            if (__0 != DeathType.Dream && __0 != DeathType.DreamExplosion && __0 != DeathType.Supernova && __0 != DeathType.TimeLoop && __0 != DeathType.Meditation)
            {
                isRespawn = true;
                __instance._isDying = true;
                __instance._deathType = __0;
                Locator.GetPauseCommandListener().AddPauseCommandLock();
                PlayerData.SetLastDeathType(__0);
                GlobalMessenger<DeathType>.FireEvent("PlayerDeath", __0);
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(DeathManager), nameof(DeathManager.FinishDeathSequence))]
        public static bool DeathManager_FinishDeathSequence(DeathManager __instance)
        {
            if (!Mod.IsInWrongWarpSystem) return true;
            if (isRespawn)
            {
                UnityUtils.DoAfterSeconds(Mod, 2f, () =>
                {
                    __instance._isDying = false;
                    GlobalMessenger.FireEvent("PlayerResurrection");
                    Locator.GetPauseCommandListener().RemovePauseCommandLock();
                    OWInput.ChangeInputMode(InputMode.Character);
                    ReticleController.Show();
                    Locator.GetPromptManager().SetPromptsVisible(true);
                    Locator.GetAudioMixer()._deathMixed = false;
                    Locator.GetAudioMixer().UnmixMemoryUplink();
                    Locator.GetPlayerBody().GetComponent<PlayerResources>().DebugRefillResources();
                    Locator.GetPlayerBody().GetComponent<PlayerResources>().enabled = true;
                    Mod.Respawner.RespawnPlayer();
                    Mod.Respawner.RespawnShip();
                });
                return false;
            }
            return true;
        }
    }
}
