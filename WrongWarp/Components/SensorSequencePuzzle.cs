using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class SensorSequencePuzzle : Sensor
    {
        public bool Activated;
        public List<Sensor> Sensors = new();

        int index;

        public override float ComputeStrength() => Activated ? 1f : 0f;

        public override void WireUp()
        {

        }

        public override void Update()
        {
            base.Update();
            if (Activated) return;

            var nextSensor = Sensors[index];

            foreach (var sensor in Sensors)
            {
                if (WasActivatedThisFrame(sensor))
                {
                    if (sensor == nextSensor)
                    {
                        index++;
                        if (index >= Sensors.Count) Activated = true;
                        return;
                    }
                    else
                    {
                        index = 0;
                        if (sensor == Sensors[0])
                        {
                            index++;
                        }
                    }
                }
            }
        }
    }
}
