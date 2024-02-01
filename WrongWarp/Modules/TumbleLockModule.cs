using ModDataTools.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Components;

namespace WrongWarp.Modules
{
    public class TumbleLockModule : WrongWarpModule
    {
        public Dictionary<string, TumbleLock> locks = new();

        public TumbleLockModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            locks.Clear();
        }

        public override void OnSystemUnload()
        {
            locks.Clear();
        }

        public TumbleLock Find(FrequencyAsset frequency)
        {
            if (locks.TryGetValue(frequency.FullID, out var tl))
            {
                return tl;
            }
            return null;
        }

        public void Register(TumbleLock tl)
        {
            locks[tl.Frequency.FullID] = tl;
        }

        public void Unregister(TumbleLock tl)
        {
            locks.Remove(tl.Frequency.FullID);
        }
    }
}
