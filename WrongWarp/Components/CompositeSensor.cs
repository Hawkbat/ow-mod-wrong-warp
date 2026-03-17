using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class CompositeSensor : Sensor
    {
        public List<Sensor> Sensors = [];
        public MergeMode Mode = MergeMode.All;
        public bool Invert = false;

        public override float ComputeStrength()
        {
            var str = Mode switch
            {
                MergeMode.All => AreAllActivated(Sensors) ? 1f : 0f,
                MergeMode.Any => AreAnyActivated(Sensors) ? 1f : 0f,
                MergeMode.Max => GetMaxStrength(Sensors),
                MergeMode.Min => GetMinStrength(Sensors),
                MergeMode.Average => GetAverageStrength(Sensors),
                _ => 0f,
            };
            return Invert ? 1f - str : str;
        }

        public override void WireUp()
        {

        }

        public enum MergeMode
        {
            All,
            Any,
            Max,
            Min,
            Average,
        }
    }
}
