using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class SignalDoorConfig : ComponentConfig
    {
        public List<string> sensorPaths;
        public string easeIn;
        public string easeOut;
        public float? speed;
        public float? gracePeriod;
    }
}
