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
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem)
            {
                return true;
            }
            if (__instance._isDying)
            {
                return false;
            }
            if (__instance._invincible && __0 != DeathType.Supernova && __0 != DeathType.BigBang && __0 != DeathType.Meditation && __0 != DeathType.TimeLoop && __0 != DeathType.BlackHole)
            {
                LogUtils.Log($"Prevented death because player is invincible (death type: {__0})");
                return false;
            }
            isRespawn = false;
            if (!WrongWarpMod.Instance.SaveData[SaveDataFlag.HasDoneIntroTour])
            {
                LogUtils.Log($"Forcing true death during intro tour (death type: {__0})");
                return true;
            }
            if (WrongWarpMod.Instance.SaveData[SaveDataFlag.RespawnDisabled])
            {
                LogUtils.Log($"Forcing true death since respawner is disabled (death type: {__0})");
                WrongWarpMod.Instance.Respawner.OverwriteFlashback();
                return true;
            }
            if (__0 == DeathType.Dream || __0 == DeathType.DreamExplosion || __0 == DeathType.Supernova || __0 == DeathType.TimeLoop || __0 == DeathType.Meditation)
            {
                LogUtils.Log($"Forcing true death because of unusual death type (death type: {__0})");
                return true;
            }

            if (__0 == DeathType.Default || __0 == DeathType.Impact)
            {
                var spawnPoint = WrongWarpMod.Instance.NewHorizonsApi.GetPlanet("WW_HEARTHIAN_EXHIBIT")
                .GetComponentsInChildren<SpawnPoint>(true)
                .FirstOrDefault(s => !s.IsShipSpawn());

                var overheatVolume = GameObject.FindObjectOfType<Components.OverheatHazardVolume>();
                var damage = overheatVolume.GetRawDamageAt(spawnPoint.transform.position);
                if (damage > overheatVolume.MinDamageThreshold)
                {
                    LogUtils.Log($"Forcing true death because overheat volume has reached spawn point (death type: {__0})");
                    return true;
                }
            }

            LogUtils.Log($"Respawning player (death type: {__0})");
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
