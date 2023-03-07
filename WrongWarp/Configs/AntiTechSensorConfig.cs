using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class AntiTechSensorConfig : ComponentConfig
    {
        public float? minDistance;
        public float? maxDistance;
    }
}
