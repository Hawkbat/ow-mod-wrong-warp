using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class SaveFlagSensor : Sensor
    {
        public SaveDataFlag Flag;

        public override float ComputeStrength()
            => Mod.SaveData[Flag] ? 1f : 0f;

        public override void WireUp()
        {

        }
    }
}
