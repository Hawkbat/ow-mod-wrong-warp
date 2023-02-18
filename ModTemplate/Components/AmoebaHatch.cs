using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class AmoebaHatch : WrongWarpBehaviour
    {
        public Sensor BlockingSensor;
        public AmoebaHatch Linked;

        [NonSerialized]
        public float SwarmPreference;
        
        private const float swarmPreferenceGainRate = 0.1f;
        private const float swarmPreferenceDecayRate = 0.1f;

        public bool IsBlocked { get => Sensor.IsActivated(BlockingSensor) || !Linked; }

        public AmoebaWaypoint Waypoint { get; protected set; }

        public override void WireUp()
        {
            Waypoint = GetComponentInParent<AmoebaWaypoint>();
            if (Waypoint) Waypoint.Hatches.Add(this);
        }

        public void OnUpdate()
        {
            var isUsed = Waypoint.Circuit.Swarms.Any() && Waypoint.Circuit.Swarms.Any(s => s.NextHatch == this);
            var prefRate = isUsed ? swarmPreferenceGainRate : swarmPreferenceDecayRate;
            SwarmPreference = Mathf.Clamp01(SwarmPreference + Time.deltaTime * prefRate);
            if (IsBlocked) SwarmPreference = 0f;
        }
        
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
}
