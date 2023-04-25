using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class WormholeModule : WrongWarpModule
    {
        GameObject whiteHole;
        GameObject blackHole;

        public WormholeModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            var core = Mod.NewHorizonsApi.GetPlanet("WW_CORE");
            blackHole = UnityUtils.GetTransformAtPath(core.transform, "./Sector/Black Hole").gameObject;
            whiteHole = UnityUtils.GetTransformAtPath(core.transform, "./Sector/White Hole").gameObject;
        }

        public override void OnSystemUnload()
        {
            blackHole = null;
            whiteHole = null;
        }

        public override void OnLateUpdate()
        {
            var blackHoleActive = Mod.SaveData.HasPlayedMuseumMelody;
            if (blackHole && !blackHole.activeSelf && blackHoleActive)
            {
                blackHole.SetActive(true);
            }
            if (whiteHole && !whiteHole.activeSelf && !blackHoleActive)
            {
                whiteHole.SetActive(true);
            }
            if (blackHole && blackHole.activeSelf && !blackHoleActive)
            {
                blackHole.SetActive(false);
            }
            if (whiteHole && whiteHole.activeSelf && blackHoleActive)
            {
                whiteHole.SetActive(false);
            }
        }
    }
}
