using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class ProbePortScreenVisitorHatchToggle : ProbePortScreen
    {
        public Transform Hatch;
        public Vector3 HatchOpenOffset;
        public Vector3 HatchClosedOffset;

        public bool IsActive => Mod.SaveData[SaveDataFlag.VisitorHatchOpen];

        public override string GetText() => $"[VISITOR_CENTER]\nMAINTAINANCE_HATCH=\n< {(IsActive ? "OPEN" : "CLOSED")} >";

        public override void WireUp()
        {
            base.WireUp();
            Hatch.localPosition = Mod.SaveData[SaveDataFlag.VisitorHatchOpen] ? HatchOpenOffset : HatchClosedOffset;
        }

        public override bool OnTick(int dx, int dy)
        {
            if (dy == 0)
            {
                var open = !Mod.SaveData[SaveDataFlag.VisitorHatchOpen];
                Mod.SaveData[SaveDataFlag.VisitorHatchOpen] = open;
                if (open)
                    Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Door_Metal_OpenStart);
                else
                    Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Door_Metal_CloseStart);
                Hatch.localPosition = open ? HatchOpenOffset : HatchClosedOffset;
                return true;
            }
            return false;
        }
    }
}
