using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Configs;

namespace WrongWarp.Components
{
    public class ExoCorpse : WrongWarpBehaviour, IConfigurable<ExoCorpseConfig>
    {
        public void ApplyConfig(ExoCorpseConfig config)
        {

        }

        public override void WireUp()
        {
            Mod.ExoCorpses.All.Add(this);
        }
    }
}
