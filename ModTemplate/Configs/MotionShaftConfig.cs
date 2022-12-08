using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class MotionShaftConfig : ComponentConfig
    {
        public List<string> sensorPaths;
        public int? indicatorCount;
        public float? indicatorSpeed;
        public float? force;
        public bool? startReversed;
    }
}
