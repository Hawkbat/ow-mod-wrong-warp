using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Configs;

namespace WrongWarp.Components
{
    public class Exhibit : WrongWarpBehaviour, IConfigurable<ExhibitConfig>
    {
        public List<Sensor> Sensors = new List<Sensor>();
        public List<string> SensorPaths;
        public string Note;

        public void ApplyConfig(ExhibitConfig config)
        {
            if (config.sensorPaths != null) SensorPaths = config.sensorPaths;
            if (config.note != null) Note = config.note;
        }

        public override void WireUp()
        {
            Sensors = GetComponentsAtPaths<Sensor>(SensorPaths);
        }

        public void Update()
        {
            if (Sensor.WereAnyActivatedThisFrame(Sensors))
            {
                Mod.Museum.TriggerNote(this);
            }
        }
    }
}
