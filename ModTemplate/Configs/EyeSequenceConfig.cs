using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Objects;

namespace WrongWarp.Configs
{
    [Serializable]
    public class EyeSequenceConfig
    {
        public XYZ travelerPosition;
        public XYZ travelerRotation;
        public XYZ altTravelerPosition;
        public XYZ altTravelerRotation;
    }
}
