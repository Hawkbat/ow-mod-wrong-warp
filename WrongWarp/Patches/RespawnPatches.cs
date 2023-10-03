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
            if (__0 == DeathType.Dream || __0 == DeathType.DreamExplosion || __0 == DeathType.Supernova || __0 == DeathType.TimeLoop || __0 == DeathType.Meditation)
            {
                return true;
            }
            isRespawn = true;
            __instance._isDying = true;
            __instance._deathType = __0;
            Locator.GetPauseCommandListener().AddPauseCommandLock();
            PlayerData.SetLastDeathType(__0);
            GlobalMessenger<DeathType>.FireEvent("PlayerDeath", __0);
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(DeathManager), nameof(DeathManager.FinishDeathSequence))]
        public static bool DeathManager_FinishDeathSequence(DeathManager __instance)
        {
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem) return true;
            if (!isRespawn)
            {
                return true;
            }
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
                var visorEffects = Locator.GetPlayerBody().GetComponentInChildren<VisorEffectController>();
                visorEffects.OnDestroy();
                visorEffects.Awake();
                var hudCanvas = Locator.GetPlayerBody().GetComponentInChildren<HUDCanvas>();
                hudCanvas.enabled = true;
                if (hudCanvas._boostArrowIndicator.GetComponentInChildren<ThrustAndAttitudeIndicator>())
                    hudCanvas._boostArrowIndicator.transform.root.GetComponentInChildren<ThrustAndAttitudeIndicator>().enabled = true;
                WrongWarpMod.Instance.Respawner.RespawnPlayer();
                WrongWarpMod.Instance.Respawner.RespawnShip();
            });
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ShipDamageController), nameof(ShipDamageController.Explode))]
        public static bool ShipDamageController_Explode(ShipDamageController __instance)
        {
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem) return true;
            LogUtils.Warn($"Prevented ship explosion");
            WrongWarpMod.Instance.Respawner.RespawnShip();
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ShipDetachableModule), nameof(ShipDetachableModule.Detach))]
        public static bool ShipDetachableModule_Detach(ShipDetachableModule __instance)
        {
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem) return true;
            LogUtils.Warn($"Prevented {__instance} from detaching");
            if (__instance.name == "Module_Cockpit")
            {
                var ejectionSystem = Locator.GetShipBody().GetComponentInChildren<ShipEjectionSystem>();
                if (ejectionSystem._ejectPressed)
                {
                    ejectionSystem._ejectPressed = false;
                    ejectionSystem.enabled = false;
                    Locator.GetPlayerAudioController().PlayNegativeUISound();
                }
                WrongWarpMod.Instance.Respawner.RespawnShip();
            }
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ShipDetachableLeg), nameof(ShipDetachableLeg.Detach))]
        public static bool ShipDetachableLeg_Detach(ShipDetachableLeg __instance)
        {
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem) return true;
            LogUtils.Warn($"Prevented {__instance} from detaching");
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ReferenceFrameGUI), nameof(ReferenceFrameGUI.OnPlayerDeath))]
        public static bool ReferenceFrameGUI_OnPlayerDeath(ReferenceFrameGUI __instance)
        {
            if (isRespawn)
            {
                return false;
            }
            return true;
        }
    }
}
