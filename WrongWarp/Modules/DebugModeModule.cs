using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using WrongWarp.Objects;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class DebugModeModule : WrongWarpModule
    {
        DebugInputMode inputMode = DebugInputMode.None;

        public DebugModeModule(WrongWarpMod mod) : base(mod)
        {
        }

        public override bool Active => true;

        public override void OnUpdate()
        {
            if (inputMode == DebugInputMode.None)
            {
                if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
                {
                    inputMode = DebugInputMode.SaveData;
                }
                if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
                {
                    inputMode = DebugInputMode.Spawn;
                }
            }
            else if (inputMode == DebugInputMode.SaveData)
            {
                if (Keyboard.current[Key.Numpad0].wasPressedThisFrame)
                {
                    inputMode = DebugInputMode.None;
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
                    Mod.SaveData.SignalJammerDisabled = !Mod.SaveData.SignalJammerDisabled;
                }
                if (Keyboard.current[Key.Numpad6].wasPressedThisFrame)
                {
                    Mod.SaveData.ExhibitRestored = !Mod.SaveData.ExhibitRestored;
                }
                if (Keyboard.current[Key.Numpad7].wasPressedThisFrame)
                {
                    Mod.SaveData.ShipSpawnChanged = !Mod.SaveData.ShipSpawnChanged;
                }
                if (Keyboard.current[Key.Numpad8].wasPressedThisFrame)
                {
                    Mod.SaveData.HasDoneIntroTour = !Mod.SaveData.HasDoneIntroTour;
                }
                if (Keyboard.current[Key.Numpad9].wasPressedThisFrame)
                {
                    Mod.SaveData.RespawnDisabled = !Mod.SaveData.RespawnDisabled;
                }
            }
            else if (inputMode == DebugInputMode.Spawn)
            {
                if (Keyboard.current[Key.Numpad0].wasPressedThisFrame)
                {
                    inputMode = DebugInputMode.None;
                }
                if (Mod.IsInWrongWarpSystem)
                {
                    if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
                    {
                        Mod.Respawner.RespawnPlayer();
                    }
                    if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
                    {
                        Mod.Respawner.RespawnShip();
                    }
                    if (Keyboard.current[Key.Numpad4].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("SPAWN_Guide");
                    }
                    if (Keyboard.current[Key.Numpad5].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("SPAWN_Archivist");
                    }
                    if (Keyboard.current[Key.Numpad6].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("SPAWN_Curator");
                    }
                    if (Keyboard.current[Key.Numpad7].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("SPAWN_Direlict");
                    }
                }
                else if (LoadManager.GetCurrentScene() == OWScene.SolarSystem)
                {
                    if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("Spawn_TH");
                    }
                    if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("Spawn_TimeLoopDevice");
                    }
                    if (Keyboard.current[Key.Numpad3].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("Spawn_Vessel");
                    }
                    if (Keyboard.current[Key.Numpad4].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("Spawn_NorthPole");
                    }
                }
                else if (LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse)
                {
                    if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("SPAWN_Vessel");
                    }
                    if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("SPAWN_Surface");
                    }
                    if (Keyboard.current[Key.Numpad3].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("SPAWN_Observatory");
                    }
                    if (Keyboard.current[Key.Numpad4].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("SPAWN_ForestOfGalaxies");
                    }
                    if (Keyboard.current[Key.Numpad5].wasPressedThisFrame)
                    {
                        WarpToSpawnPoint("SPAWN_Campfire");
                    }
                }
            }
            if (Keyboard.current[Key.NumpadPeriod].wasPressedThisFrame)
            {
                if (Locator.GetPlayerSuit().IsWearingSuit())
                    Locator.GetPlayerSuit().RemoveSuit();
                else
                    Locator.GetPlayerSuit().SuitUp();
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

        public override void OnGUI()
        {
            if (inputMode == DebugInputMode.None)
            {
                GUILayout.Label($"{nameof(Key.Numpad1)} = SaveData Debug Menu");
                GUILayout.Label($"{nameof(Key.Numpad2)} = Spawn Debug Menu");
            } else if (inputMode == DebugInputMode.SaveData)
            {
                GUILayout.Label($"{nameof(Key.Numpad0)} = Exit Debug Menu");
                GUILayout.Label($"{nameof(Key.Numpad1)} = {nameof(Mod.SaveData.ArchivistSignalActive)} ({Mod.SaveData.ArchivistSignalActive})");
                GUILayout.Label($"{nameof(Key.Numpad2)} = {nameof(Mod.SaveData.GuideSignalActive)} ({Mod.SaveData.GuideSignalActive})");
                GUILayout.Label($"{nameof(Key.Numpad3)} = {nameof(Mod.SaveData.CuratorSignalActive)} ({Mod.SaveData.CuratorSignalActive})");
                GUILayout.Label($"{nameof(Key.Numpad4)} = {nameof(Mod.SaveData.HasPlayedMuseumMelody)} ({Mod.SaveData.HasPlayedMuseumMelody})");
                GUILayout.Label($"{nameof(Key.Numpad5)} = {nameof(Mod.SaveData.SignalJammerDisabled)} ({Mod.SaveData.SignalJammerDisabled})");
                GUILayout.Label($"{nameof(Key.Numpad6)} = {nameof(Mod.SaveData.ExhibitRestored)} ({Mod.SaveData.ExhibitRestored})");
                GUILayout.Label($"{nameof(Key.Numpad7)} = {nameof(Mod.SaveData.ShipSpawnChanged)} ({Mod.SaveData.ShipSpawnChanged})");
                GUILayout.Label($"{nameof(Key.Numpad8)} = {nameof(Mod.SaveData.HasDoneIntroTour)} ({Mod.SaveData.HasDoneIntroTour})");
                GUILayout.Label($"{nameof(Key.Numpad9)} = {nameof(Mod.SaveData.RespawnDisabled)} ({Mod.SaveData.RespawnDisabled})");
            }
            else if (inputMode == DebugInputMode.Spawn)
            {
                GUILayout.Label($"{nameof(Key.Numpad0)} = Exit Debug Menu");
                if (Mod.IsInWrongWarpSystem)
                {
                    GUILayout.Label($"{nameof(Key.Numpad1)} = Respawn Player");
                    GUILayout.Label($"{nameof(Key.Numpad2)} = Respawn Ship");
                    GUILayout.Label($"{nameof(Key.Numpad4)} = SPAWN_Guide");
                    GUILayout.Label($"{nameof(Key.Numpad5)} = SPAWN_Archivist");
                    GUILayout.Label($"{nameof(Key.Numpad6)} = SPAWN_Curator");
                    GUILayout.Label($"{nameof(Key.Numpad7)} = SPAWN_Direlict");
                }
                else if (LoadManager.GetCurrentScene() == OWScene.SolarSystem)
                {
                    GUILayout.Label($"{nameof(Key.Numpad1)} = Spawn_TH");
                    GUILayout.Label($"{nameof(Key.Numpad2)} = Spawn_TimeLoopDevice");
                    GUILayout.Label($"{nameof(Key.Numpad3)} = Spawn_Vessel");
                    GUILayout.Label($"{nameof(Key.Numpad4)} = Spawn_NorthPole");
                }
                else if (LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse)
                {
                    GUILayout.Label($"{nameof(Key.Numpad1)} = SPAWN_Vessel");
                    GUILayout.Label($"{nameof(Key.Numpad2)} = SPAWN_Surface");
                    GUILayout.Label($"{nameof(Key.Numpad3)} = SPAWN_Observatory");
                    GUILayout.Label($"{nameof(Key.Numpad4)} = SPAWN_ForestOfGalaxies");
                    GUILayout.Label($"{nameof(Key.Numpad5)} = SPAWN_Campfire");
                }
            }
            GUILayout.Label($"{nameof(Key.NumpadPeriod)} = Suit Up/Remove Suit");
            GUILayout.Label($"{nameof(Key.NumpadMinus)} = {nameof(Mod.Warp.WarpToHearthianSystem)}");
            GUILayout.Label($"{nameof(Key.NumpadPlus)} = {nameof(Mod.Warp.WarpToEye)}");
            GUILayout.Label($"{nameof(Key.NumpadEnter)} = {nameof(Mod.Warp.WarpToWrongWarpSystem)}");
        }

        void WarpToSpawnPoint(string spawnPointName)
        {
            var spawner = Locator.GetPlayerBody().GetComponent<PlayerSpawner>();
            var spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
            LogUtils.Log(string.Join(", ", spawnPoints.Select(s => s.name)));
            var spawnPoint = spawnPoints.First(s => s.name == spawnPointName);
            if (spawnPoint is EyeSpawnPoint eyeSpawn)
            {
                Locator.GetEyeStateManager().SetState(eyeSpawn.GetEyeState());
            }
            spawner.DebugWarp(spawnPoint);
        }

        public enum DebugInputMode
        {
            None = 0,
            SaveData = 1,
            Spawn = 2,
        }
    }
}
