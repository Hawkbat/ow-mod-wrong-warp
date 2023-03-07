using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Configs
{
    [Serializable]
    public class ObjectTypeConfig
    {
        public AntiTechSensorConfig antiTechSensor = null;
        public BioSensorConfig bioSensor = null;
        public ExhibitConfig exhibit = null;
        public ExoCorpseConfig exoCorpse = null;
        public HoloGuideConfig holoGuide = null;
        public HoloPlanetConfig holoPlanet = null;
        public MotionShaftConfig motionShaft = null;
        public VanillaPropConfig prop = null;
        public QuantumEntityStateConfig quantumEntityState = null;
        public ScanPulseConfig scanPulse = null;
        public SignalBarrierConfig signalBarrier = null;
        public SignalDoorConfig signalDoor = null;
        public SignalEmitterConfig signalEmitter = null;
        public SignalLightConfig signalLight = null;
    }
}
