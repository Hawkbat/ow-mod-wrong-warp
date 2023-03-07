using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;

namespace WrongWarp.Components
{
    public class AntiTechSensor : Sensor, IConfigurable<AntiTechSensorConfig>
    {

        public void ApplyConfig(AntiTechSensorConfig config)
        {
            if (config.minDistance.HasValue) MinDistance = config.minDistance.Value;
            if (config.maxDistance.HasValue) MaxDistance = config.maxDistance.Value;
        }

        public override void WireUp()
        {

        }

        public override float ComputeStrength()
        {
            var strength = 0f;
            var player = Locator.GetPlayerBody();
            if (player && PlayerState.IsWearingSuit())
            {
                strength = Mathf.Max(strength, GetDistanceStrength(player.transform));
            }
            var probe = Locator.GetProbe();
            if (probe && probe.IsLaunched())
            {
                strength = Mathf.Max(strength, GetDistanceStrength(probe.transform));
            }
            var ship = Locator.GetShipBody();
            if (ship)
            {
                strength = Mathf.Max(strength, GetDistanceStrength(ship.transform));
            }
            return strength;
        }

        float GetDistanceStrength(Transform other)
        {
            var dist = Vector3.Distance(transform.position, other.position);
            var str = Mathf.Clamp01(Mathf.InverseLerp(MaxDistance, MinDistance, dist));
            return str;
        }
    }
}
