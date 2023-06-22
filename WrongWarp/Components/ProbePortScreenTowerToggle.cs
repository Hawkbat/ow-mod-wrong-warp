using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalTowerType = WrongWarp.Modules.SignalTowerModule.SignalTowerType;

namespace WrongWarp.Components
{
    public class ProbePortScreenTowerToggle : ProbePortScreen
    {
        public SignalTowerType SignalTower;

        public bool IsActive => Mod.SignalTowers.IsSignalTowerActive(SignalTower);

        public override string GetText() => $"[SIGNAL TRACKING]\n{Mod.SignalTowers.GetSignalTowerStatusText(SignalTower)}\n< {(IsActive ? "ENABLED" : "DISABLED")} >";

        public override bool OnTick(int dx, int dy)
        {
            if (dy == 0)
            {
                Mod.SignalTowers.SetSignalTowerActive(SignalTower, !IsActive);
                return true;
            }
            return false;
        }
    }
}
