using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;
using ScreenType = WrongWarp.Components.ProbePort.ScreenType;

namespace WrongWarp.Modules
{
    public class ProbePortModule : WrongWarpModule
    {

        public ProbePortModule(WrongWarpMod mod) : base(mod)
        {

        }

        public string GetScreenText(ScreenType screenType)
        {
            switch (screenType)
            {
                case ScreenType.SignalTowers:
                    return Mod.SignalTowers.GetNotificationText();
                case ScreenType.PlayerStatus:
                    {
                        var oxygen = Locator.GetPlayerController()._playerResources.GetOxygenInSeconds();
                        var health = Locator.GetPlayerController()._playerResources.GetHealthFraction();
                        return $"Visitor Vitals\nOxygen Remaining: {oxygen}\nBiological Integrity: {Math.Round(health * 100)}%";
                    }
                case ScreenType.Temperature:
                    {
                        var minTemp = 750f;
                        var maxTemp = 5778f;
                        var i = EasingUtils.EaseIn(TimeLoop.GetFractionElapsed(), EasingUtils.Quartic);
                        var temp = Mathf.RoundToInt(Mathf.Lerp(minTemp, maxTemp, i));
                        return $"Core Temperature\n{temp}K";
                    }
                default:
                    return "Data Corrupted";
            }
        }
    }
}
