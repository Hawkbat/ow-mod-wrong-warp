using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class ScanPulseConfig : ComponentConfig
    {
        public float? duration;
        public float? maxRadius;
        public string easeIn;
        public string easeOut;
    }
}
