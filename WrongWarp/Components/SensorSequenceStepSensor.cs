using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class SensorSequenceStepSensor : Sensor
    {
        public SensorSequencePuzzle Sequence;
        public int Step;

        public override float ComputeStrength()
            => Sequence.Activated || Sequence.GetCurrentStep() > Step ? 1f : 0f;

        public override void WireUp()
        {

        }
    }
}
