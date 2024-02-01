using ModDataTools.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class ProbePortScreenTumbleLock : ProbePortScreen
    {
        public FrequencyAsset Frequency;

        public override string GetText()
        {
            var tumbleLock = Mod.TumbleLocks.Find(Frequency);
            if (tumbleLock == null)
            {
                return $"[ARCHIVES ACCESS]\n{Frequency.FullName}\nERROR: CONNECTION LOST";
            }
            return $"[ARCHIVES ACCESS]\n{Frequency.FullName}\n< {tumbleLock.RotationIndex + 1}/{tumbleLock.Rotations.Count} >";
        }

        public override bool OnTick(int dx, int dy)
        {
            if (dy == 0)
            {
                var tumbleLock = Mod.TumbleLocks.Find(Frequency);
                if (tumbleLock == null) return false;
                var delta = dx < 0 ? -1 : 1;
                var i = (tumbleLock.RotationIndex + delta + tumbleLock.Rotations.Count) % tumbleLock.Rotations.Count;
                tumbleLock.RotationIndex = i;
                return true;
            }
            return false;
        }
    }
}
