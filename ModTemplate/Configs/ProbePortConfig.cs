using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Components;
using ScreenType = WrongWarp.Components.ProbePort.ScreenType;
using EntityType = WrongWarp.Components.ProbePort.ProbeGame.EntityType;

namespace WrongWarp.Configs
{
    [Serializable]
    public class ProbePortConfig : ComponentConfig
    {
        public List<ScreenConfig> screens;

        [Serializable]
        public class ScreenConfig
        {
            public ScreenType type;
            public int? x;
            public int? y;
            public ProbeGameScreenConfig probeGame;
        }

        [Serializable]
        public class ProbeGameScreenConfig
        {
            public List<EntityConfig> entities;

            [Serializable]
            public class EntityConfig
            {
                public int x = 0;
                public int y = 0;
                public int w = 1;
                public int h = 1;
                public EntityType type = EntityType.Unknown;
            }
        }
    }
}
