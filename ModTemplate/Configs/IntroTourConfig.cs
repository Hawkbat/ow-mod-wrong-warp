using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Objects;

namespace WrongWarp.Configs
{
    [Serializable]
    public class IntroTourConfig
    {
        public VesselConfig vessel = new VesselConfig();

        [Serializable]
        public class VesselConfig
        {
            public XYZ acceleration;
            public XYZ angularAcceleration;
        }
    }
}
