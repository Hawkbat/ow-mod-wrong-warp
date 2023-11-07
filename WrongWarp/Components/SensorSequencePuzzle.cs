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
                        LogUtils.Log($"Correct Signal Activated ({index}/{Sensors.Count}) - {name}");
                        if (index >= Sensors.Count)
                        {
                            Activated = true;
                            LogUtils.Log($"Sequence Complete - {name}");
                            Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.NomaiTractorBeamActivate);
                        }
                        return;
                    }
                    else
                    {
                        index = 0;
                        LogUtils.Log($"Wrong Signal Activated - {name}");
                        Locator.GetPlayerAudioController()._oneShotExternalSource.PlayOneShot(AudioType.NomaiTractorBeamDeactivate);
                        if (sensor == Sensors[0])
                        {
                            index++;
                            LogUtils.Log($"Correct Signal Activated ({index}/{Sensors.Count}) - {name}");
                        }
                    }
                }
            }
        }
    }
}
