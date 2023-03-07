using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Components;

namespace WrongWarp.Modules
{
    public class ExoCorpseModule : WrongWarpModule
    {
        public List<ExoCorpse> All = new List<ExoCorpse>();

        public ExoCorpseModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {

        }

        public override void OnSystemUnload()
        {
            All.Clear();
        }
    }
}
