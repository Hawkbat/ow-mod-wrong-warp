using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;

namespace WrongWarp.Components
{
    public class BioSensor : RadialSensor, IConfigurable<BioSensorConfig>
    {

        public void ApplyConfig(BioSensorConfig config)
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
            var playerDistance = Vector3.Distance(transform.position, Locator.GetPlayerBody().transform.position);
            var playerStrength = Mathf.Clamp01(Mathf.InverseLerp(MaxDistance, MinDistance, playerDistance));
            strength = Mathf.Max(strength, playerStrength);
            foreach (var exoCorpse in Mod.ExoCorpses.All)
            {
                if (!exoCorpse) continue;
                var corpseDistance = Vector3.Distance(transform.position, exoCorpse.transform.position);
                var corpseStrength = Mathf.Clamp01(Mathf.InverseLerp(MaxDistance, MinDistance, corpseDistance));
                strength = Mathf.Max(strength, corpseStrength);
            }
            return strength;
        }
    }
}
