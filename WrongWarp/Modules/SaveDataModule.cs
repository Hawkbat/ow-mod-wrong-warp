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
    public class SaveDataModule(WrongWarpMod mod) : WrongWarpModule(mod)
    {
        private const string SAVE_CONDITION_PREFIX = "WW_SAVE_";

        public bool this[SaveDataFlag flag]
        {
            get => PlayerData.GetPersistentCondition(SAVE_CONDITION_PREFIX + flag.ToString());
            set {
                var oldValue = this[flag];
                if (value != oldValue)
                {
                    LogUtils.Success($"Changing {flag} from {oldValue} to {value}");
                    PlayerData.SetPersistentCondition(SAVE_CONDITION_PREFIX + flag.ToString(), value);
                }
            }
        }

        public override bool Active => true;

        public void ResetAllFlags()
        {
            foreach (SaveDataFlag flag in Enum.GetValues(typeof(SaveDataFlag)))
            {
                this[flag] = false;
            }
            LogUtils.Success("All save data flags have been reset.");
        }
    }
}
