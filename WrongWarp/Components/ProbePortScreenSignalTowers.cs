using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class ProbePortScreenSignalTowers : ProbePortScreen
    {
        public override string GetText() => Mod.SignalTowers.GetProbePortText();
    }
}
