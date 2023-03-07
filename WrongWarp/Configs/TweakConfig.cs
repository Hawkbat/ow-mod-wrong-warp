using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Objects;

namespace WrongWarp.Configs
{
    [Serializable]
    public class TweakConfig
    {
        public RGBA flashbackColor;
        public List<string> museumNotes = new List<string>();
        public IntroTourConfig introTour = new IntroTourConfig();
        public EyeSequenceConfig eyeSequence = new EyeSequenceConfig();
        public Dictionary<string, ObjectTypeConfig> objectTypes = new Dictionary<string, ObjectTypeConfig>();
        public Dictionary<string, SignalConfig> signalPool = new Dictionary<string, SignalConfig>();
    }
}
