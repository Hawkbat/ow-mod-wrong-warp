﻿using HarmonyLib;
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
            }
            else if (WrongWarpMod.Instance.Heat.IsActiveNotification(__instance.Data))
            {
                __instance.TextToDisplay = WrongWarpMod.Instance.Heat.GetNotificationText();
            }
            else
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

        [HarmonyPostfix, HarmonyPatch(typeof(NomaiText), nameof(NomaiText.GetTextNode), [typeof(int)])]
        public static void NomaiText_GetTextNode(NomaiText __instance, ref string __result)
        {
            var exoCorpse = __instance.gameObject.GetComponentInParent<ExoCorpse>();
            if (exoCorpse != null)
            {
                var shouldScramble = exoCorpse.IsScrambled;
                var scrambling = true;
                var ignoringTag = false;
                var s = string.Empty;
                if (shouldScramble)
                {
                    s += "<color=grey>";
                }
                for (int i = 0; i < __result.Length; i++)
                {
                    var c = __result[i];
                    var skipChar = false;
                    if (c == '~')
                    {
                        skipChar = true;
                        if (scrambling)
                        {
                            if (shouldScramble) s += "</color>";
                            scrambling = false;
                        }
                        else
                        {
                            if (shouldScramble) s += "<color=grey>";
                            scrambling = true;
                        }
                    }
                    else if (c == '<')
                    {
                        ignoringTag = true;
                    }
                    else if (c == '>')
                    {
                        ignoringTag = false;
                        if (scrambling) skipChar = true;
                    }
                    else if (shouldScramble && scrambling && !ignoringTag && !char.IsWhiteSpace(c))
                    {
                        var ascii = (byte)UnityEngine.Random.Range(32, 127);
                        c = Encoding.ASCII.GetString([ascii])[0];
                    }
                    if (ignoringTag && scrambling) skipChar = true;
                    if (!skipChar)
                    {
                        s += c;
                    }
                }
                if (shouldScramble && scrambling)
                {
                    s += "</color>";
                }
                __result = s;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(NomaiTranslatorProp), nameof(NomaiTranslatorProp.DisplayTextNode))]
        public static void NomaiTranslatorProp_DisplayTextNode(NomaiTranslatorProp __instance)
        {
            if (!WrongWarpMod.Instance.IsInWrongWarpSystem) return;
            if (__instance._translationTimeElapsed == 0f && !__instance._nomaiTextComponent.IsTranslated(__instance._currentTextID))
            {
                __instance._textField.text = WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI("UNTRANSLATED_EXO_TEXT");
            }
        }
    }
}
