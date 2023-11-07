using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using WrongWarp.Objects;
using WrongWarp.Utils;

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

        public bool SignalJammerDisabled
        {
            get => data.jammerActive;
            set => SetAndSave(ref data.jammerActive, nameof(SignalJammerDisabled), value);
        }

        public bool ExhibitRestored
        {
            get => data.exhibitRestored;
            set => SetAndSave(ref data.exhibitRestored, nameof(ExhibitRestored), value);
        }

        public bool RespawnDisabled
        {
            get => data.respawnDisabled;
            set => SetAndSave(ref data.respawnDisabled, nameof(RespawnDisabled), value);
        }

        private void SetAndSave<T>(ref T data, string name, T value) where T : IEquatable<T>
        {
            if (!value.Equals(data))
            {
                LogUtils.Success($"Changing {name} from {data} to {value}");
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
                LogUtils.Success($"Created new {nameof(WrongWarp)} save data for profile {GetCurrentProfile()}");
                Save(true);
            } else
            {
                LogUtils.Success($"Loaded {nameof(WrongWarp)} save data for profile {GetCurrentProfile()}");
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
            LogUtils.Success($"Saved {nameof(WrongWarp)} save data for profile {GetCurrentProfile()}");
        }

        private void Reset()
        {
            data = new SaveData()
            {
                initialized = true
            };
            LogUtils.Success($"Reset {nameof(WrongWarp)} save data for profile {GetCurrentProfile()}");
            Save(true);
        }

        private string GetCurrentProfile() => StandaloneProfileManager.SharedInstance?.currentProfile?.profileName ?? "XboxGamepassDefaultProfile";
        private string GetCurrentSaveFileName() => SAVE_PATH + GetCurrentProfile() + ".json";
    }
}
