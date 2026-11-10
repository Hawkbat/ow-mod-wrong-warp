using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class TumbleLockRotationSensor : Sensor
    {
        public TumbleLock Lock;
        public int[] ValidIndices;

        public override void WireUp()
        {

        }

        public override float ComputeStrength()
        {
            if (!Lock || ValidIndices.Length == 0) return 0f;
            foreach (var index in ValidIndices)
            {
                if (Lock.IsAtTarget(index))
                {
                    return 1f;
                }
            }
            return 0f;
        }
    }
}