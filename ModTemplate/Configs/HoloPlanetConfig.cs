using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class HoloPlanetConfig : ComponentConfig
    {
        public string planet;
        public string centerBody;
        public float? scaleFactor;
        public float? distanceFactor;
    }
}
