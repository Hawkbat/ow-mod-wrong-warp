using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class CleanupOnDestroy : WrongWarpBehaviour
    {
        OxygenVolume oxygenVolume;

        public override void WireUp()
        {
            oxygenVolume = GetComponent<OxygenVolume>();
        }

        protected void OnDestroy()
        {
            if (oxygenVolume != null)
            {
                oxygenVolume.OnEffectVolumeExit(Locator.GetPlayerDetector());
            }
        }
    }
}
