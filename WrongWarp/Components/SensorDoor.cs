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
    public class SensorDoor : WrongWarpBehaviour, IConfigurable<SignalDoorConfig>
    {
        public List<Sensor> Sensors = new List<Sensor>();
        public List<string> SensorPaths = new List<string>();
        public EasingFunc EaseIn;
        public EasingFunc EaseOut;
        public float Speed;
        public float GracePeriod;

        public Transform Pivot;
        public Transform StartPoint;
        public Transform EndPoint;

        public float OpenAmount = 0f;
        public bool IsOpen = false;
        public float OpenTime = 0f;

        public bool IsOpening => IsOpen && OpenAmount < 1f;
        public bool IsClosing => !IsOpen && OpenAmount > 0f;
        public bool ShouldOpen => !IsOpen && !IsOpening && Sensor.AreAllActivated(Sensors);
        public bool ShouldClose => IsOpen && !IsClosing && Sensor.AreAnyDeactivated(Sensors) && OpenTime > GracePeriod;

        public void ApplyConfig(SignalDoorConfig config)
        {
            if (config.sensorPaths != null) SensorPaths = config.sensorPaths;
            if (config.easeIn != null) EaseIn = EasingUtils.Lookup(config.easeIn);
            if (config.easeOut != null) EaseOut = EasingUtils.Lookup(config.easeOut);
            if (config.speed.HasValue) Speed = config.speed.Value;
            if (config.gracePeriod.HasValue) GracePeriod = config.gracePeriod.Value;
        }

        public override void WireUp()
        {
            if (!Sensors.Any()) Sensors = GetComponentsAtPaths<Sensor>(SensorPaths);
            if (!Pivot) Pivot = GetTransformAtPath("Pivot");
            if (!StartPoint) StartPoint = GetTransformAtPath("Start");
            if (!EndPoint) EndPoint = GetTransformAtPath("End");
        }

        public void Update()
        {
            if (ShouldOpen) IsOpen = true;
            if (ShouldClose) IsOpen = false;
            if (IsOpen) OpenTime += Time.deltaTime;
            else OpenTime = 0f;
            OpenAmount = Mathf.MoveTowards(OpenAmount, IsOpen ? 1f : 0f, Time.deltaTime * Speed);
            if (Pivot && StartPoint && EndPoint)
            {
                float t = EasingUtils.Ease(OpenAmount, IsOpen ? EaseIn : EaseOut, IsOpen ? EaseOut : EaseIn);
                Pivot.transform.position = Vector3.Lerp(StartPoint.position, EndPoint.position, t);
            }
        }
    }
}
