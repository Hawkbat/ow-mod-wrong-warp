using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Objects
{
    [Serializable]
    public class SaveData
    {
        public bool initialized = false;
        public string profile = "";
        public bool wrongWarpTaken = false;
        public bool hasDoneIntroTour = false;
        public bool shipSpawnChanged = false;
        public bool archivistSignalActive = false;
        public bool curatorSignalActive = false;
        public bool guideSignalActive = false;
        public bool playedMuseumMelody = false;
        public bool signalJammerDisabled = false;
        public bool exhibitRestored = false;
        public bool respawnDisabled = false;

        public bool this[SaveDataFlag flag]
        {
            get => flag switch
            {
                SaveDataFlag.ArchivistSignalActive => archivistSignalActive,
                SaveDataFlag.GuideSignalActive => guideSignalActive,
                SaveDataFlag.CuratorSignalActive => curatorSignalActive,
                SaveDataFlag.HasPlayedMuseumMelody => playedMuseumMelody,
                SaveDataFlag.SignalJammerDisabled => signalJammerDisabled,
                SaveDataFlag.ExhibitRestored => exhibitRestored,
                SaveDataFlag.ShipSpawnChanged => shipSpawnChanged,
                SaveDataFlag.HasDoneIntroTour => hasDoneIntroTour,
                SaveDataFlag.RespawnDisabled => respawnDisabled,
                SaveDataFlag.WrongWarpTaken => wrongWarpTaken,
                _ => throw new ArgumentOutOfRangeException(nameof(flag)),
            };
            set {
                _ = flag switch
                {
                    SaveDataFlag.ArchivistSignalActive => archivistSignalActive = value,
                    SaveDataFlag.GuideSignalActive => guideSignalActive = value,
                    SaveDataFlag.CuratorSignalActive => curatorSignalActive = value,
                    SaveDataFlag.HasPlayedMuseumMelody => playedMuseumMelody = value,
                    SaveDataFlag.SignalJammerDisabled => signalJammerDisabled = value,
                    SaveDataFlag.ExhibitRestored => exhibitRestored = value,
                    SaveDataFlag.ShipSpawnChanged => shipSpawnChanged = value,
                    SaveDataFlag.HasDoneIntroTour => hasDoneIntroTour = value,
                    SaveDataFlag.RespawnDisabled => respawnDisabled = value,
                    SaveDataFlag.WrongWarpTaken => wrongWarpTaken = value,
                    _ => throw new ArgumentOutOfRangeException(nameof(flag)),
                };
            }
        }
    }
}
