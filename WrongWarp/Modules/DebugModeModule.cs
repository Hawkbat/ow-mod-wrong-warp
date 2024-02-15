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
        bool takingScreenshot;

        public DebugModeModule(WrongWarpMod mod) : base(mod)
        {
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.Numpad0, () => $"Exit {inputMode} Menu", () => inputMode = DebugInputMode.None, () => inputMode != DebugInputMode.None));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.Numpad1, () => "SaveData Debug Menu", () => inputMode = DebugInputMode.SaveData, null));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.Numpad2, () => "Spawn Debug Menu", () => inputMode = DebugInputMode.Spawn, null));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.Numpad3, () => "Screenshot Debug Menu", () => inputMode = DebugInputMode.Screenshot, null));

            AddSaveDataCommand(Key.Numpad1, SaveDataFlag.ArchivistSignalActive);
            AddSaveDataCommand(Key.Numpad2, SaveDataFlag.GuideSignalActive);
            AddSaveDataCommand(Key.Numpad3, SaveDataFlag.CuratorSignalActive);
            AddSaveDataCommand(Key.Numpad4, SaveDataFlag.HasPlayedMuseumMelody);
            AddSaveDataCommand(Key.Numpad5, SaveDataFlag.SignalJammerDisabled);
            AddSaveDataCommand(Key.Numpad6, SaveDataFlag.ExhibitRestored);
            AddSaveDataCommand(Key.Numpad7, SaveDataFlag.ShipSpawnChanged);
            AddSaveDataCommand(Key.Numpad8, SaveDataFlag.HasDoneIntroTour);
            AddSaveDataCommand(Key.Numpad9, SaveDataFlag.RespawnDisabled);

            commands.Add(new DebugCommand(this, DebugInputMode.Spawn, Key.Numpad1, () => "Respawn Player", () => Mod.Respawner.RespawnPlayer(), () => Mod.IsInWrongWarpSystem));
            commands.Add(new DebugCommand(this, DebugInputMode.Spawn, Key.Numpad2, () => "Respawn Ship", () => Mod.Respawner.RespawnShip(), () => Mod.IsInWrongWarpSystem));
            
            AddSpawnCommand(Key.Numpad4, "SPAWN_Guide", () => Mod.IsInWrongWarpSystem);
            AddSpawnCommand(Key.Numpad5, "SPAWN_Archivist", () => Mod.IsInWrongWarpSystem);
            AddSpawnCommand(Key.Numpad6, "SPAWN_Curator", () => Mod.IsInWrongWarpSystem);
            AddSpawnCommand(Key.Numpad7, "SPAWN_Direlict", () => Mod.IsInWrongWarpSystem);

            AddSpawnCommand(Key.Numpad1, "Spawn_TH", () => LoadManager.GetCurrentScene() == OWScene.SolarSystem);
            AddSpawnCommand(Key.Numpad2, "Spawn_TimeLoopDevice", () => LoadManager.GetCurrentScene() == OWScene.SolarSystem);
            AddSpawnCommand(Key.Numpad3, "Spawn_Vessel", () => LoadManager.GetCurrentScene() == OWScene.SolarSystem);
            AddSpawnCommand(Key.Numpad4, "Spawn_NorthPole", () => LoadManager.GetCurrentScene() == OWScene.SolarSystem);

            AddSpawnCommand(Key.Numpad1, "SPAWN_Vessel", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);
            AddSpawnCommand(Key.Numpad2, "SPAWN_Surface", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);
            AddSpawnCommand(Key.Numpad3, "SPAWN_Observatory", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);
            AddSpawnCommand(Key.Numpad4, "SPAWN_ForestOfGalaxies", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);
            AddSpawnCommand(Key.Numpad5, "SPAWN_Campfire", () => LoadManager.GetCurrentScene() == OWScene.EyeOfTheUniverse);

            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadPeriod, () => "Remove Suit", () => Locator.GetPlayerSuit().RemoveSuit(), () => Locator.GetPlayerSuit()?.IsWearingSuit() ?? false));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadPeriod, () => "Suit Up", () => Locator.GetPlayerSuit().SuitUp(), () => !Locator.GetPlayerSuit()?.IsWearingSuit() ?? false));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadMinus, () => "Warp to Hearthian System", () => Mod.Warp.WarpToHearthianSystem(), null));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadPlus, () => "Warp to Eye of the Universe", () => Mod.Warp.WarpToEye(), null));
            commands.Add(new DebugCommand(this, DebugInputMode.None, Key.NumpadEnter, () => "Warp to Warp Warp System", () => Mod.Warp.WarpToWrongWarpSystem(), null));

            commands.Add(new DebugCommand(this, DebugInputMode.Screenshot, Key.Numpad1, () => "Take Screenshot", () => TakeShipLogScreenshot(), null));

            void AddSaveDataCommand(Key key, SaveDataFlag flag)
            {
                commands.Add(new DebugCommand(this, DebugInputMode.SaveData, key, () => $"{flag} ({Mod.SaveData[flag]})", () => Mod.SaveData[flag] = !Mod.SaveData[flag], null));
            }

            void AddSpawnCommand(Key key, string spawnPoint, Func<bool> condition)
            {
                commands.Add(new DebugCommand(this, DebugInputMode.Spawn, key, () => spawnPoint, () => WarpToSpawnPoint(spawnPoint), condition));
            }
        }

        public override bool Active => true;

        public override void OnUpdate()
        {
            foreach (var cmd in commands)
            {
                if (cmd.ShouldExecute())
                {
                    cmd.Execute();
                }
            }
        }

        public override void OnGUI()
        {
            foreach (var cmd in commands)
            {
                if (!cmd.IsActive()) continue;
                GUILayout.Label(cmd.ToString());
            }
            if (inputMode == DebugInputMode.Screenshot && takingScreenshot)
            {
                var x = (Screen.width - 512f) / 2f;
                var y = (Screen.height - 512f) / 2f;
                GUI.Box(new Rect(x, y, 512f, 512f), string.Empty);
            }
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

        void TakeShipLogScreenshot()
        {
            Mod.StartCoroutine(DoTakeShipLogScreenshot());
        }

        IEnumerator DoTakeShipLogScreenshot()
        {
            takingScreenshot = true;
            var previousRenderMode = GUIMode._renderMode;
            GUIMode.SetRenderMode(GUIMode.RenderMode.Hidden);
            var newFOV = 120f;
            Locator.GetPlayerCameraController().SnapToFieldOfView(newFOV, 0.5f);
            yield return new WaitForSeconds(0.5f);
            yield return null;
            yield return new WaitForEndOfFrame();
            var x = (Screen.width - 512f) / 2f;
            var y = (Screen.height - 512f) / 2f;
            var tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(x, y, 512f, 512f), 0, 0);
            tex.Apply();
            var colors = tex.GetPixels();
            for (int i = 0; i < colors.Length ; i++)
            {
                var c = colors[i];
                var intensity = 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;
                c.r = c.g = c.b = intensity;
                colors[i] = c;
            }
            tex.SetPixels(colors);
            tex.Apply();
            var bytes = tex.EncodeToPNG();
            var path = $"{Mod.ModHelper.Manifest.ModFolderPath}/Screenshots/{DateTime.Now:yyyy-MM-dd HH-mm-ss}.png";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            System.IO.File.WriteAllBytes(path, bytes);
            Locator.GetPlayerAudioController().PlayProbeSnapshot();
            Locator.GetPlayerCameraController().SnapToInitFieldOfView(0.5f);
            GUIMode.SetRenderMode(previousRenderMode);
            takingScreenshot = false;
        }

        public enum DebugInputMode
        {
            None = 0,
            SaveData = 1,
            Spawn = 2,
            Screenshot = 3,
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

            public bool IsActive() => module.inputMode == mode && IsConditionMet();
            public bool IsConditionMet() => condition == null || condition();
            public bool ShouldExecute() => IsActive() && Keyboard.current[key].wasPressedThisFrame;

            public void Execute()
            {
                action?.Invoke();
            }

            public override string ToString() => $"{key} = {label()}";
        }
    }
}
