using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class SignalEmitterConfig : ComponentConfig
    {
        public string signal;
        public string signalPrefix;
        public string signalSuffix;
        public float? allocRange;
        public float? deallocRange;
    }
}
