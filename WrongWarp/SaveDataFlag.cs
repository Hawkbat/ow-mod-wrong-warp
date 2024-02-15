using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp
{
    public enum SaveDataFlag
    {
        None = 0,
        ArchivistSignalActive = 1,
        GuideSignalActive = 2,
        CuratorSignalActive = 3,
        HasPlayedMuseumMelody = 4,
        SignalJammerDisabled = 5,
        ExhibitRestored = 6,
        ShipSpawnChanged = 7,
        HasDoneIntroTour = 8,
        RespawnDisabled = 9,
        WrongWarpTaken = 10,
    }
}
