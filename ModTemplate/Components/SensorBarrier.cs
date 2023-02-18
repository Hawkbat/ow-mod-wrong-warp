using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;
using WrongWarp.Modules;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class SensorBarrier : WrongWarpBehaviour, IConfigurable<SignalBarrierConfig>
    {
        public List<Sensor> Sensors = new List<Sensor>();
        public List<string> SensorPaths = new List<string>();
        public List<Transform> Barriers = new List<Transform>();
        public List<string> BarrierPaths = new List<string>();

        public bool IsOn = false;

        public void ApplyConfig(SignalBarrierConfig config)
        {
            if (config.sensorPaths != null) SensorPaths = config.sensorPaths;
            if (config.barrierPaths != null) BarrierPaths = config.barrierPaths;
        }

        public override void WireUp()
        {
            Sensors = GetComponentsAtPaths<Sensor>(SensorPaths);
            Barriers = GetComponentsAtPaths<Transform>(BarrierPaths);
        }

        public void Update()
        {
            IsOn = Sensor.AreAllActivated(Sensors);
            foreach (var barrier in Barriers)
            {
                barrier.gameObject.SetActive(IsOn);
            }
        }
    }
}
