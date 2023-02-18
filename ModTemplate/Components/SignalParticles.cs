using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class SignalParticles : WrongWarpBehaviour
    {
        public List<Sensor> Sensors = new List<Sensor>();

        ParticleSystem ps;
        float rotMult;

        public override void WireUp()
        {
            ps = GetComponent<ParticleSystem>();
            rotMult = ps.emission.rateOverTimeMultiplier;
        }

        public void Update()
        {
            if (!ps) return;
            var emission = ps.emission;
            emission.rateOverTimeMultiplier = rotMult * Sensor.GetMaxStrength(Sensors);
        }
    }
}
