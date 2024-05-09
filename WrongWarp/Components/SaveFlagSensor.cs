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
        public bool Invert;

        public override float ComputeStrength()
        {
            var str = Mod.SaveData[Flag] ? 1f : 0f;
            if (Invert) str = 1f - str;
            return str;
        }

        public override void WireUp()
        {

        }
    }
}
