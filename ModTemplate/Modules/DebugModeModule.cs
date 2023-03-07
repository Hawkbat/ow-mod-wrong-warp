using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using WrongWarp.Objects;

namespace WrongWarp.Modules
{
    public class DebugModeModule : WrongWarpModule
    {
        public DebugModeModule(WrongWarpMod mod) : base(mod)
        {
        }

        public override bool Active => true;

        public override void OnUpdate()
        {
            if (Mod.IsInWrongWarpSystem)
            {
                if (Keyboard.current[Key.Numpad0].wasPressedThisFrame)
                {
                    Mod.SaveData.HasDoneIntroTour = !Mod.SaveData.HasDoneIntroTour;
                }
                if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
                {
                    Mod.SaveData.ArchivistSignalActive = !Mod.SaveData.ArchivistSignalActive;
                }
                if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
                {
                    Mod.SaveData.GuideSignalActive = !Mod.SaveData.GuideSignalActive;
                }
                if (Keyboard.current[Key.Numpad3].wasPressedThisFrame)
                {
                    Mod.SaveData.CuratorSignalActive = !Mod.SaveData.CuratorSignalActive;
                }
                if (Keyboard.current[Key.Numpad4].wasPressedThisFrame)
                {
                    Mod.SaveData.HasPlayedMuseumMelody = !Mod.SaveData.HasPlayedMuseumMelody;
                }
                if (Keyboard.current[Key.Numpad5].wasPressedThisFrame)
                {
                    Mod.SaveData.SignalJammerActive = !Mod.SaveData.SignalJammerActive;
                }
                if (Keyboard.current[Key.Numpad6].wasPressedThisFrame)
                {
                    Mod.SaveData.ExhibitRestored = !Mod.SaveData.ExhibitRestored;
                }
                if (Keyboard.current[Key.Numpad7].wasPressedThisFrame)
                {
                    Mod.SaveData.ShipSpawnChanged = !Mod.SaveData.ShipSpawnChanged;
                }
            } else if (LoadManager.GetCurrentScene() == OWScene.SolarSystem)
            {
                if (Keyboard.current[Key.Numpad0].wasPressedThisFrame)
                {
                    WarpToSpawnPoint("Spawn_TH");
                }
                if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
                {
                    WarpToSpawnPoint("Spawn_TimeLoopDevice");
                }
                if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
                {
                    WarpToSpawnPoint("Spawn_Vessel");
                }
                if (Keyboard.current[Key.Numpad3].wasPressedThisFrame)
                {
                    WarpToSpawnPoint("Spawn_NorthPole");
                }
                if (Keyboard.current[Key.Numpad4].wasPressedThisFrame)
                {
                    if (Locator.GetPlayerSuit().IsWearingSuit())
                        Locator.GetPlayerSuit().RemoveSuit();
                    else
                        Locator.GetPlayerSuit().SuitUp();
                }
            } else if (LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse)
            {
                if (Keyboard.current[Key.Numpad0].wasPressedThisFrame)
                {
                    WarpToSpawnPoint("SPAWN_Vessel");
                }
                if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
                {
                    WarpToSpawnPoint("SPAWN_Surface");
                }
                if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
                {
                    WarpToSpawnPoint("SPAWN_Observatory");
                }
                if (Keyboard.current[Key.Numpad3].wasPressedThisFrame)
                {
                    WarpToSpawnPoint("SPAWN_ForestOfGalaxies");
                }
                if (Keyboard.current[Key.Numpad4].wasPressedThisFrame)
                {
                    WarpToSpawnPoint("SPAWN_Campfire");
                }
            }

            if (Keyboard.current[Key.NumpadMinus].wasPressedThisFrame)
            {
                Mod.Warp.WarpToHearthianSystem();
            }
            if (Keyboard.current[Key.NumpadPlus].wasPressedThisFrame)
            {
                Mod.Warp.WarpToEye();
            }
            if (Keyboard.current[Key.NumpadEnter].wasPressedThisFrame)
            {
                Mod.Warp.WarpToWrongWarpSystem();
            }
        }

        void WarpToSpawnPoint(string spawnPointName)
        {
            var spawner = Locator.GetPlayerBody().GetComponent<PlayerSpawner>();
            var spawnPoint = spawner._spawnList.FirstOrDefault(s => s.name == spawnPointName);
            if (spawnPoint is EyeSpawnPoint eyeSpawn)
            {
                Locator.GetEyeStateManager().SetState(eyeSpawn.GetEyeState());
            }
            spawner.DebugWarp(spawnPoint);
        }
    }
}
