using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class ProbePortScreenSignalscopeToggle : ProbePortScreen
    {
        public bool IsActive => Mod.SaveData.SignalJammerDisabled;

        public override string GetText()
            => $"[QUANTUM SIGNALSCOPE]\n< {(IsActive ? "ENABLED" : "DISABLED")} >";

        public override bool OnTick(int dx, int dy)
        {
            if (dy == 0)
            {
                Mod.SaveData.SignalJammerDisabled = !Mod.SaveData.SignalJammerDisabled;
                return true;
            }
            return false;
        }
    }
}
