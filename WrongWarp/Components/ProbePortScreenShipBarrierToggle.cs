using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class ProbePortScreenShipBarrierToggle : ProbePortScreen
    {
        public bool IsActive => Mod.SaveData[SaveDataFlag.ShipBarrierDisabled];

        public override string GetText() => $"[EXHIBIT_ENCLOSURE]\nOBJ_SHIP_HEARTHIAN=\n< {(IsActive ? "DISABLED" : "ENABLED")} >";

        public override bool OnTick(int dx, int dy)
        {
            if (dy == 0)
            {
                Mod.SaveData[SaveDataFlag.ShipBarrierDisabled] = !Mod.SaveData[SaveDataFlag.ShipBarrierDisabled];
                return true;
            }
            return false;
        }
    }
}
