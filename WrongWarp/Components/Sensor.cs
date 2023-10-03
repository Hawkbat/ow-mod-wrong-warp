using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public abstract class Sensor : WrongWarpBehaviour
    {
        public float Strength { get; private set; }
        public float PreviousStrength { get; private set; }

        public abstract float ComputeStrength();

        public virtual void Update()
        {
            PreviousStrength = Strength;
            Strength = ComputeStrength();
        }

        public static float GetStrength(Sensor sensor) => sensor != null ? Mathf.Clamp01(sensor.Strength) : 0f;
        public static float GetPreviousStrength(Sensor sensor) => sensor != null ? Mathf.Clamp01(sensor.PreviousStrength) : 0f;
        public static bool IsActivated(Sensor sensor) => GetStrength(sensor) == 1f;
        public static bool IsDeactivated(Sensor sensor) => GetStrength(sensor) < 1f;
        public static bool WasDeactivatedThisFrame(Sensor sensor) => GetStrength(sensor) < 1f && GetPreviousStrength(sensor) == 1f;
        public static bool WasActivatedThisFrame(Sensor sensor) => GetStrength(sensor) == 1f && GetPreviousStrength(sensor) < 1f;
        public static float GetMaxStrength(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() ? sensors.Max(e => GetStrength(e)) : 0f;
        public static float GetMinStrength(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() ? sensors.Min(e => GetStrength(e)) : 0f;
        public static bool AreAnyActivated(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() && sensors.Any(s => IsActivated(s));
        public static bool AreAllActivated(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() && sensors.All(s => IsActivated(s));
        public static bool AreAnyDeactivated(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() && sensors.Any(s => IsDeactivated(s));
        public static bool AreAllDeactivated(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() && sensors.All(s => IsActivated(s));
        public static bool WereAnyActivatedThisFrame(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() && sensors.Any(e => WasActivatedThisFrame(e));
        public static bool WereAllActivatedThisFrame(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() && sensors.All(e => WasActivatedThisFrame(e));
        public static bool WereAnyDeactivatedThisFrame(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() && sensors.Any(e => WasDeactivatedThisFrame(e));
        public static bool WereAllDeactivatedThisFrame(IEnumerable<Sensor> sensors) => sensors != null && sensors.Any() && sensors.All(e => WasDeactivatedThisFrame(e));
    }
}
