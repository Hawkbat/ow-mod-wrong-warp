using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class WarpModule : WrongWarpModule
    {
        public WarpModule(WrongWarpMod mod) : base(mod)
        {
            Mod.ModHelper.Events.Player.OnPlayerAwake += Player_OnPlayerAwake;
        }

        public override void OnSystemLoad()
        {
            
        }

        public override void OnSystemUnload()
        {

        }

        private void Player_OnPlayerAwake(PlayerBody obj)
        {
            /*DoAfterFrames(5, () =>
            {
                if (Mod.SaveData.WrongWarpTaken && !Mod.IsInWrongWarpSystem)
                {
                    WarpToWrongWarpSystem();
                }
            });*/
        }

        public bool CheckWrongWarpCoordinates(NomaiCoordinateInterface coords)
        {
            var warpCoordinates = Mod.TweakConfig.warpCoordinates;
            if (warpCoordinates.Count != 3 || warpCoordinates.Any(l => l.Count == 0))
            {
                Mod.ModHelper.Console.WriteLine("Invalid warp coordinates configured");
                return false;
            }
            bool checkX = coords._nodeControllers[0].CheckCoordinate(warpCoordinates[0].ToArray());
            bool checkY = coords._nodeControllers[1].CheckCoordinate(warpCoordinates[1].ToArray());
            bool checkZ = coords._nodeControllers[2].CheckCoordinate(warpCoordinates[2].ToArray());
            bool allMatch = checkX && checkY && checkZ;
            Mod.ModHelper.Console.WriteLine($"Checking wrong warp coordinates: {allMatch}", MessageType.Info);
            return allMatch;
        }

        public void WarpToWrongWarpSystem()
        {
            WarpToWrongWarpSystem(!Mod.SaveData.HasDoneIntroTour);
        }

        public void WarpToWrongWarpSystem(bool doIntroTour)
        {
            Mod.ModHelper.Console.WriteLine($"Attempting to warp to {WrongWarpMod.SOLAR_SYSTEM_NAME}", MessageType.Info);
            Mod.SaveData.WrongWarpTaken = true;
            if (doIntroTour) Mod.SaveData.HasDoneIntroTour = false;
            Mod.NewHorizonsApi.SetDefaultSystem(WrongWarpMod.SOLAR_SYSTEM_NAME);
            Mod.NewHorizonsApi.ChangeCurrentStarSystem(WrongWarpMod.SOLAR_SYSTEM_NAME);
        }

        public void WarpToHearthianSystem()
        {
            if (PlayerData.GetWarpedToTheEye()) PlayerData.SaveEyeCompletion();
            Mod.ModHelper.Console.WriteLine($"Attempting to warp to {OWScene.SolarSystem}", MessageType.Info);
            Mod.SaveData.WrongWarpTaken = false;
            Mod.ModHelper.Events.Unity.FireInNUpdates(() =>
            {
                Mod.NewHorizonsApi.SetDefaultSystem(nameof(OWScene.SolarSystem));
                Mod.NewHorizonsApi.ChangeCurrentStarSystem(nameof(OWScene.SolarSystem));
            }, 2);
        }

        public void WarpToEye()
        {
            if (!PlayerData.GetWarpedToTheEye()) PlayerData.SaveWarpedToTheEye(TimeLoop.GetSecondsRemaining());
            Mod.ModHelper.Console.WriteLine($"Attempting to warp to {OWScene.EyeOfTheUniverse}", MessageType.Info);
            Mod.SaveData.WrongWarpTaken = false;
            Mod.ModHelper.Events.Unity.FireInNUpdates(() =>
            {
                Mod.NewHorizonsApi.SetDefaultSystem(nameof(OWScene.SolarSystem));
                Mod.NewHorizonsApi.ChangeCurrentStarSystem(nameof(OWScene.EyeOfTheUniverse));
            }, 2);
        }
    }
}
