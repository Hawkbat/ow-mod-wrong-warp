﻿using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using WrongWarp.Objects;

namespace WrongWarp.Modules
{
    public class SaveDataModule : WrongWarpModule
    {
        private const string SAVE_PATH = "saves/";

        private SaveData data = null;
        private bool saveNextFrame;

        public bool WrongWarpTaken
        {
            get => data.wrongWarpTaken;
            set => SetAndSave(ref data.wrongWarpTaken, nameof(WrongWarpTaken), value);
        }

        public bool HasDoneIntroTour
        {
            get => data.doneIntroTour;
            set => SetAndSave(ref data.doneIntroTour, nameof(HasDoneIntroTour), value);
        }

        public bool ShipSpawnChanged
        {
            get => data.shipSpawnChanged;
            set => SetAndSave(ref data.shipSpawnChanged, nameof(ShipSpawnChanged), value);
        }

        public bool ArchivistSignalActive
        {
            get => data.archivistSignalActive;
            set => SetAndSave(ref data.archivistSignalActive, nameof(ArchivistSignalActive), value);
        }

        public bool GuideSignalActive
        {
            get => data.guideSignalActive;
            set => SetAndSave(ref data.guideSignalActive, nameof(GuideSignalActive), value);
        }
        public bool CuratorSignalActive
        {
            get => data.curatorSignalActive;
            set => SetAndSave(ref data.curatorSignalActive, nameof(CuratorSignalActive), value);
        }

        public bool HasPlayedMuseumMelody
        {
            get => data.playedMuseumMelody;
            set => SetAndSave(ref data.playedMuseumMelody, nameof(HasPlayedMuseumMelody), value);
        }

        public bool SignalJammerActive
        {
            get => data.jammerActive;
            set => SetAndSave(ref data.jammerActive, nameof(SignalJammerActive), value);
        }

        public bool ExhibitRestored
        {
            get => data.exhibitRestored;
            set => SetAndSave(ref data.exhibitRestored, nameof(ExhibitRestored), value);
        }

        private void SetAndSave<T>(ref T data, string name, T value) where T : IEquatable<T>
        {
            if (!value.Equals(data))
            {
                Mod.ModHelper.Console.WriteLine($"Changing {name} from {data} to {value}", MessageType.Success);
                data = value;
                Save();
            }
        }

        public override bool Active => true;

        public SaveDataModule(WrongWarpMod mod) : base(mod) {
            LoadManager.OnCompleteSceneLoad += LoadManager_OnCompleteSceneLoad;
        }

        public override void OnSystemLoad()
        {
            Load();
        }

        public override void OnSystemUnload()
        {
            Save();
        }

        public override void OnLateUpdate()
        {
            if (saveNextFrame)
            {
                saveNextFrame = false;
                Save(true);
            }
        }

        private void LoadManager_OnCompleteSceneLoad(OWScene originalScene, OWScene loadScene)
        {
            if (loadScene == OWScene.SolarSystem || loadScene == OWScene.EyeOfTheUniverse)
            {
                Load();
            }
        }

        private void Load()
        {
            if (this.data != null && this.data.initialized && this.data.profile == GetCurrentProfile())
            {
                return;
            }
            var data = Mod.ModHelper.Storage.Load<SaveData>(GetCurrentSaveFileName());
            if (data == null || !data.initialized || data.profile != GetCurrentProfile())
            {
                data = new SaveData
                {
                    initialized = true,
                    profile = GetCurrentProfile(),
                };
                Mod.ModHelper.Console.WriteLine($"Created new {nameof(WrongWarp)} save data for profile {GetCurrentProfile()}", MessageType.Success);
                Save(true);
            } else
            {
                Mod.ModHelper.Console.WriteLine($"Loaded {nameof(WrongWarp)} save data for profile {GetCurrentProfile()}", MessageType.Success);
            }
            this.data = data;
        }

        private void Save(bool immediately = false)
        {
            if (data == null) return;
            if (!immediately)
            {
                saveNextFrame = true;
                return;
            }
            Mod.ModHelper.Storage.Save(data, GetCurrentSaveFileName());
            Mod.ModHelper.Console.WriteLine($"Saved {nameof(WrongWarp)} save data for profile {GetCurrentProfile()}", MessageType.Success);
        }

        private void Reset()
        {
            data = new SaveData()
            {
                initialized = true
            };
            Mod.ModHelper.Console.WriteLine($"Reset {nameof(WrongWarp)} save data for profile {GetCurrentProfile()}", MessageType.Success);
            Save(true);
        }

        private string GetCurrentProfile() => StandaloneProfileManager.SharedInstance?.currentProfile?.profileName ?? "XboxGamepassDefaultProfile";
        private string GetCurrentSaveFileName() => SAVE_PATH + GetCurrentProfile() + ".json";
    }
}