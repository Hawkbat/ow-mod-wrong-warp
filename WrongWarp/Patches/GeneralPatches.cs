using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Components;
using WrongWarp.Utils;

namespace WrongWarp.Patches
{
    [HarmonyPatch]
    public static class GeneralPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(TestTagScript), nameof(TestTagScript.Awake))]
        public static bool TestTagScript_Awake(TestTagScript __instance)
        {
            if (__instance.transform.parent && __instance.gameObject.name.StartsWith("$"))
            {
                LogUtils.Warn($"Old TestTagScript hijack still in place at {UnityUtils.GetTransformPath(__instance.transform)}");
                WrongWarpMod.Instance.ApplyModObjectType(__instance.transform.parent.gameObject, __instance.gameObject.name.Substring(1));
                UnityEngine.Object.Destroy(__instance.gameObject);
                return false;
            }
            return true;
        }


        [HarmonyPrefix, HarmonyPatch(typeof(NotificationDisplay.NotificationDisplayData), nameof(NotificationDisplay.NotificationDisplayData.IncrementTimeDisplayed))]
        public static void NotificationDisplayData_IncrementTimeDisplayed(NotificationDisplay.NotificationDisplayData __instance)
        {
            bool isRealtimeNotification = true;
            if (WrongWarpMod.Instance.SignalTowers.IsActiveNotification(__instance.Data))
            {
                __instance.TextToDisplay = WrongWarpMod.Instance.SignalTowers.GetNotificationText();
            } else if (WrongWarpMod.Instance.Heat.IsActiveNotification(__instance.Data))
            {
                __instance.TextToDisplay = WrongWarpMod.Instance.Heat.GetNotificationText();
            } else
            {
                isRealtimeNotification = false;
            }
            if (isRealtimeNotification)
            {
                __instance.TextDisplay.text = __instance.TextToDisplay;
                __instance.TextScrollEffect._strToDisplay = __instance.TextToDisplay;
                __instance.TextScrollEffect.SetTypingTime(TextSpeed.Instant);
                __instance.TextScrollEffect.CompleteTextEffect();
            }
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

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerData), nameof(PlayerData.GetFreezeTimeWhileReadingTranslator))]
        public static void PlayerData_GetFreezeTimeWhileReadingTranslator(ref bool __result)
        {
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem) return;
            if (__result)
            {
                if (ExoText.All.Any(t => t.IsHighlighted))
                {
                    __result = false;
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(NomaiTextLine), nameof(NomaiTextLine.DetermineTextLineColor))]
        public static void NomaiTextLine_DetermineTextLineColor(NomaiTextLine __instance, ref Color __result)
        {
            var exoText = ExoText.All.FirstOrDefault(t => t.TextLine == __instance);
            if (exoText)
            {
                if (__instance._state != NomaiTextLine.VisualState.HIDDEN)
                {
                    __result = Sensor.IsActivated(exoText) ? exoText.ActiveColor : exoText.InactiveColor;
                }
            }
        }
    }
}
