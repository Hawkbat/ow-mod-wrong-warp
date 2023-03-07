using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Patches
{
    [HarmonyPatch]
    public static class RespawnPatches
    {
        static bool isRespawn = false;

        [HarmonyPrefix, HarmonyPatch(typeof(DeathManager), nameof(DeathManager.KillPlayer))]
        public static bool DeathManager_KillPlayer(DeathManager __instance, DeathType __0)
        {
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem) return true;
            if (!WrongWarpMod.Instance.SaveData.HasDoneIntroTour) return true;
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
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem) return true;
            if (isRespawn)
            {
                UnityUtils.DoAfterSeconds(WrongWarpMod.Instance, 2f, () =>
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
                    WrongWarpMod.Instance.Respawner.RespawnPlayer();
                    WrongWarpMod.Instance.Respawner.RespawnShip();
                });
                return false;
            }
            return true;
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
            __instance._entryCardTemplate = UnityEngine.Object.Instantiate(shipLogDetectiveModeEntryCardTemplate);
            __instance._entryLinkTemplate = UnityEngine.Object.Instantiate(shipLogDetectiveModeEntryLinkTemplate);
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ShipLogDetectiveMode), nameof(ShipLogDetectiveMode.Initialize))]
        public static void ShipLogDetectiveMode_Initialize_Postfix(ShipLogDetectiveMode __instance)
        {
            __instance._entryCardTemplate = UnityEngine.Object.Instantiate(shipLogDetectiveModeEntryCardTemplate);
            __instance._entryLinkTemplate = UnityEngine.Object.Instantiate(shipLogDetectiveModeEntryLinkTemplate);
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
    }
}
