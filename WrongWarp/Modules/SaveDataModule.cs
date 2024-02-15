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

        public bool this[SaveDataFlag flag]
        {
            get => data[flag];
            set {
                var oldValue = data[flag];
                if (!value.Equals(oldValue))
                {
                    LogUtils.Success($"Changing {flag} from {oldValue} to {value}");
                    data[flag] = value;
                    Save();
                }
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
