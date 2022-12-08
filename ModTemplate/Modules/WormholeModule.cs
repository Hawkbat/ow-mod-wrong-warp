using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Modules
{
    public class WormholeModule : WrongWarpModule
    {
        GameObject whiteHole;
        GameObject blackHole;

        public WormholeModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            blackHole = Mod.NewHorizonsApi.GetPlanet(Mod.TweakConfig.wormhole.blackHole);
            whiteHole = Mod.NewHorizonsApi.GetPlanet(Mod.TweakConfig.wormhole.whiteHole);
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
                ResetPlanet(blackHole);
            }
            if (whiteHole && !whiteHole.activeSelf && !blackHoleActive)
            {
                whiteHole.SetActive(true);
                ResetPlanet(whiteHole);
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

        private void ResetPlanet(GameObject planet)
        {
            var core = Mod.NewHorizonsApi.GetPlanet("Core");
            var coreBody = core.GetComponent<OWRigidbody>();
            var body = planet.GetComponent<OWRigidbody>();
            if (coreBody && body)
            {
                body.SetPosition(coreBody.GetPosition());
                body.SetVelocity(coreBody.GetVelocity());
            }
        }
    }
}
