using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Objects;

namespace WrongWarp.Configs
{
    [Serializable]
    public class NHPlanetConfig
    {
        public string name;
        public PropsConfig Props;

        [Serializable]
        public class PropsConfig
        {
            public List<PropDetailConfig> details;

            [Serializable]
            public class PropDetailConfig
            {
                public string assetBundle;
                public string path;
                public string rename;
                public ObjectTypeConfig modObjectType;
                public List<string> modObjectTypeNames;
            }
        }
    }
}
