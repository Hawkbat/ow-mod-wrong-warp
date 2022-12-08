using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class HoloGuideConfig : ComponentConfig
    {
        public string characterPath;
        public float? characterHeight;
    }
}
