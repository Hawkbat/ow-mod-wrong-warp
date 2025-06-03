using System;
using System.Collections;
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
        readonly List<DebugCommand> commands = [];
        bool visible = true;

        public DebugModeModule(WrongWarpMod mod) : base(mod)
        {
            commands.Add(new DebugCommand(this, DebugInputMode.Any, Key.Numpad0, () => $"Exit {inputMode} Menu", () => inputMode = DebugInputMode.None, () => inputMode != DebugInputMode.None));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.Numpad1, () => "SaveData Debug Menu", () => inputMode = DebugInputMode.SaveData, null));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.Numpad2, () => "Spawn Debug Menu", () => inputMode = DebugInputMode.Spawn, null));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.Numpad3, () => "Warp Debug Menu", () => inputMode = DebugInputMode.Warp, null));

            AddSaveDataCommand(Key.Numpad1, SaveDataFlag.ArchivistSignalActive);
            AddSaveDataCommand(Key.Numpad2, SaveDataFlag.GuideSignalActive);
            AddSaveDataCommand(Key.Numpad3, SaveDataFlag.CuratorSignalActive);
            AddSaveDataCommand(Key.Numpad4, SaveDataFlag.HasPlayedMuseumMelody);
            AddSaveDataCommand(Key.Numpad5, SaveDataFlag.SignalJammerDisabled);
            AddSaveDataCommand(Key.Numpad6, SaveDataFlag.ExhibitRestored);
            AddSaveDataCommand(Key.Numpad7, SaveDataFlag.ShipSpawnChanged);
            AddSaveDataCommand(Key.Numpad8, SaveDataFlag.HasDoneIntroTour);
            AddSaveDataCommand(Key.Numpad9, SaveDataFlag.RespawnDisabled);
            AddSaveDataCommand(Key.NumpadDivide, SaveDataFlag.ShipBarrierDisabled);
            AddSaveDataCommand(Key.NumpadMultiply, SaveDataFlag.VisitorHatchOpen);

            AddSpawnCommand(Key.Numpad1, "PlayerSpawnPoint", () => Mod.IsInWrongWarpSystem);
            AddSpawnCommand(Key.Numpad3, "SPAWN_Guide", () => Mod.IsInWrongWarpSystem);
            AddSpawnCommand(Key.Numpad4, "SPAWN_Archivist_North", () => Mod.IsInWrongWarpSystem);
            AddSpawnCommand(Key.Numpad5, "SPAWN_Archivist_South", () => Mod.IsInWrongWarpSystem);
            AddSpawnCommand(Key.Numpad6, "SPAWN_Archivist_Interior", () => Mod.IsInWrongWarpSystem);
            AddSpawnCommand(Key.Numpad7, "SPAWN_Curator", () => Mod.IsInWrongWarpSystem);
            AddSpawnCommand(Key.Numpad8, "SPAWN_Direlict", () => Mod.IsInWrongWarpSystem);

            AddSpawnCommand(Key.Numpad1, "Spawn_TH", () => !Mod.IsInWrongWarpSystem && LoadManager.GetCurrentScene() == OWScene.SolarSystem);
            AddSpawnCommand(Key.Numpad2, "Spawn_TimeLoopDevice", () => !Mod.IsInWrongWarpSystem && LoadManager.GetCurrentScene() == OWScene.SolarSystem);
            AddSpawnCommand(Key.Numpad3, "Spawn_Vessel", () => !Mod.IsInWrongWarpSystem && LoadManager.GetCurrentScene() == OWScene.SolarSystem);
            AddSpawnCommand(Key.Numpad4, "Spawn_NorthPole", () => !Mod.IsInWrongWarpSystem && LoadManager.GetCurrentScene() == OWScene.SolarSystem);

            AddSpawnCommand(Key.Numpad1, "SPAWN_Vessel", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);
            AddSpawnCommand(Key.Numpad2, "SPAWN_Surface", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);
            AddSpawnCommand(Key.Numpad3, "SPAWN_Observatory", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);
            AddSpawnCommand(Key.Numpad4, "SPAWN_ForestOfGalaxies", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);
            AddSpawnCommand(Key.Numpad5, "SPAWN_Campfire", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);

            commands.Add(new DebugCommand(this, DebugInputMode.Warp, Key.Numpad1, () => "Warp to Hearthian System", () => Mod.Warp.WarpToHearthianSystem(), null));
            commands.Add(new DebugCommand(this, DebugInputMode.Warp, Key.Numpad2, () => "Warp to Eye of the Universe", () => Mod.Warp.WarpToEye(), null));
            commands.Add(new DebugCommand(this, DebugInputMode.Warp, Key.Numpad3, () => "Warp to Wrong Warp System", () => Mod.Warp.WarpToWrongWarpSystem(false), null));
            commands.Add(new DebugCommand(this, DebugInputMode.Warp, Key.Numpad4, () => "Warp to Wrong Warp System (Force Intro Tour)", () => Mod.Warp.WarpToWrongWarpSystem(true), null));

            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadPeriod, () => "Remove Suit", () => Locator.GetPlayerSuit().RemoveSuit(), () => Locator.GetPlayerSuit()?.IsWearingSuit() ?? false));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadPeriod, () => "Suit Up", () => Locator.GetPlayerSuit().SuitUp(), () => !Locator.GetPlayerSuit()?.IsWearingSuit() ?? false));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadDivide, () => "Kill Player", () => Locator.GetDeathManager().KillPlayer(DeathType.Default), null));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadMultiply, () => "Refill Resources", () => Locator.GetPlayerTransform().GetComponent<PlayerResources>().DebugRefillResources(), null));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadPlus, () => "Respawn Player", () => Mod.Respawner.RespawnPlayer(), () => Mod.IsInWrongWarpSystem));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadMinus, () => "Respawn Ship", () => Mod.Respawner.RespawnShip(), () => Mod.IsInWrongWarpSystem));

            commands.Add(new DebugCommand(this, DebugInputMode.Any, Key.NumpadEnter, () => "Hide Debug Menu", () => visible = !visible, null));

            void AddSaveDataCommand(Key key, SaveDataFlag flag)
            {
                commands.Add(new DebugCommand(this, DebugInputMode.SaveData, key, () => $"{flag} ({Mod.SaveData[flag]})", () => Mod.SaveData[flag] = !Mod.SaveData[flag], null));
            }

            void AddSpawnCommand(Key key, string spawnPoint, Func<bool> condition)
            {
                commands.Add(new DebugCommand(this, DebugInputMode.Spawn, key, () => spawnPoint, () => WarpToSpawnPoint(spawnPoint), condition));
            }
        }

        public override bool Active => LoadManager.GetCurrentScene() == OWScene.SolarSystem || LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse;

        public override void OnUpdate()
        {
            foreach (var cmd in commands)
            {
                if (cmd.ShouldExecute())
                {
                    cmd.Execute();
                    break;
                }
            }
        }

        public override void OnGUI()
        {
            if (!visible) return;
            foreach (var cmd in commands)
            {
                if (!cmd.IsActive()) continue;
                GUILayout.Label(cmd.ToString());
            }
        }

        void WarpToSpawnPoint(string spawnPointName)
        {
            var spawner = Locator.GetPlayerBody().GetComponent<PlayerSpawner>();
            var spawnPoint = spawner._spawnList.FirstOrDefault(s => s.name == spawnPointName);
            if (!spawnPoint)
            {
                var spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
                LogUtils.Log(string.Join(", ", spawnPoints.Select(s => s.name)));
                spawnPoint = spawnPoints.First(s => s.name == spawnPointName);
            }
            if (spawnPoint is EyeSpawnPoint eyeSpawn)
            {
                Locator.GetEyeStateManager().SetState(eyeSpawn.GetEyeState());
            }
            spawner.DebugWarp(spawnPoint);
        }

        public enum DebugInputMode
        {
            Any = -1,
            None = 0,
            SaveData = 1,
            Spawn = 2,
            Warp = 3,
        }

        public class DebugCommand
        {
            public readonly DebugModeModule module;
            public readonly DebugInputMode mode;
            public readonly Key key;
            public readonly Func<string> label;
            public readonly Action action;
            public readonly Func<bool> condition;

            public DebugCommand(DebugModeModule module, DebugInputMode mode, Key key, Func<string> label, Action action, Func<bool> condition)
            {
                this.module = module;
                this.mode = mode;
                this.key = key;
                this.label = label;
                this.action = action;
                this.condition = condition;
            }

            public bool IsActive() => IsModeActive() && IsConditionMet();
            public bool IsModeActive() => mode == DebugInputMode.Any || module.inputMode == mode;
            public bool IsConditionMet() => condition == null || condition();
            public bool ShouldExecute() => IsActive() && Keyboard.current[key].wasPressedThisFrame;
            public void Execute() => action?.Invoke();

            public override string ToString() => $"{key} = {label()}";
        }
    }
}
