using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class ProbePortScreenTemperature : ProbePortScreen
    {
        public override string GetText()
        {
            var minTemp = 750f;
            var maxTemp = 5778f;
            var i = EasingUtils.EaseIn(TimeLoop.GetFractionElapsed(), EasingUtils.Quartic);
            var temp = Mathf.RoundToInt(Mathf.Lerp(minTemp, maxTemp, i));
            return $"[Core Temperature]\n{temp}K";
        }
    }
}
