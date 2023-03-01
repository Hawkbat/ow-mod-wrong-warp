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
        public bool wrongWarpTaken = false;
        public bool doneIntroTour = false;
        public bool shipSpawnChanged = false;
        public bool archivistSignalActive = false;
        public bool curatorSignalActive = false;
        public bool guideSignalActive = false;
        public bool playedMuseumMelody = false;
        public bool jammerActive = false;
        public bool exhibitRestored = false;
    }
}
