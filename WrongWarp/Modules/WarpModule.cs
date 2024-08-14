using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using WrongWarp.NewHorizons;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class WarpModule : WrongWarpModule
    {
        public WarpModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            
        }

        public override void OnSystemUnload()
        {

        }

        public void WarpToWrongWarpSystem(bool forceIntroTour)
        {
            if (PlayerData.GetWarpedToTheEye()) PlayerData.SaveEyeCompletion();
            LogUtils.Log($"Attempting to warp to {WrongWarpMod.SOLAR_SYSTEM_NAME}");
            Mod.NewHorizonsApi.SetDefaultSystem(WrongWarpMod.SOLAR_SYSTEM_NAME);
            if (forceIntroTour)
            {
                NewHorizonsReflection.ChangeCurrentStarSystem(WrongWarpMod.SOLAR_SYSTEM_NAME, false, true);
            }
            else
            {
                Mod.NewHorizonsApi.ChangeCurrentStarSystem(WrongWarpMod.SOLAR_SYSTEM_NAME);
            }
        }

        public void WarpToHearthianSystem()
        {
            if (PlayerData.GetWarpedToTheEye()) PlayerData.SaveEyeCompletion();
            LogUtils.Log($"Attempting to warp to {OWScene.SolarSystem}");
            UnityUtils.DoAfterFrames(Mod, 2, () =>
            {
                Mod.NewHorizonsApi.SetDefaultSystem(nameof(OWScene.SolarSystem));
                Mod.NewHorizonsApi.ChangeCurrentStarSystem(nameof(OWScene.SolarSystem));
            }, true);
        }

        public void WarpToEye()
        {
            if (!PlayerData.GetWarpedToTheEye()) PlayerData.SaveWarpedToTheEye(TimeLoop.GetSecondsRemaining());
            LogUtils.Log($"Attempting to warp to {OWScene.EyeOfTheUniverse}");
            UnityUtils.DoAfterFrames(Mod, 2, () =>
            {
                Mod.NewHorizonsApi.SetDefaultSystem(nameof(OWScene.SolarSystem));
                Mod.NewHorizonsApi.ChangeCurrentStarSystem(nameof(OWScene.EyeOfTheUniverse));
            }, true);
        }
    }
}
