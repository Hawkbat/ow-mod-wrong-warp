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
        public float supernovaTime;
        public VesselConfig vessel = new VesselConfig();
        public PlayerConfig player = new PlayerConfig();

        [Serializable]
        public class VesselConfig
        {
            public XYZ position;
            public XYZ rotation;
            public XYZ acceleration;
            public XYZ angularAcceleration;
        }

        [Serializable]
        public class PlayerConfig
        {
            public XYZ offset;
            public XYZ rotation;
        }
    }
}
