using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class SignalBarrierConfig : ComponentConfig
    {
        public List<string> sensorPaths;
        public List<string> barrierPaths;
    }
}
