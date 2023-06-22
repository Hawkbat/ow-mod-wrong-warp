using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class ProbePortScreenSpawnToggle : ProbePortScreen
    {
        public bool IsActive => Mod.SaveData.ShipSpawnChanged;

        public override string GetText() => $"[EXHIBIT_OWNERSHIP]\nOBJ_SHIP_HEARTHIAN=\n< {(IsActive ? "EXH_SEEKER" : "EXH_HEARTHIAN")} >{(Mod.Respawner.IsRespawningShip ? "\nPROCESSING..." : "")}";

        public override bool OnTick(int dx, int dy)
        {
            if (dy == 0)
            {
                if (Mod.Respawner.IsRespawningShip) return false;
                Mod.SaveData.ShipSpawnChanged = !Mod.SaveData.ShipSpawnChanged;
                Mod.Respawner.RespawnShip();
                return true;
            }
            return false;
        }
    }
}
