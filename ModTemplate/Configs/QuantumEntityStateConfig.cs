using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class QuantumEntityStateConfig : ComponentConfig
    {
        public string group;
        public bool? isInitialState;
        public float? entangleRadius;
        public bool? needsSignalJammer;
    }
}
