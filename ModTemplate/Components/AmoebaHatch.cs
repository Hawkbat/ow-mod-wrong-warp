using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class AmoebaHatch : WrongWarpBehaviour
    {
        public Sensor BlockingSensor;
        public AmoebaHatch Linked;

        public bool IsBlocked { get => Sensor.IsActivated(BlockingSensor) || !Linked; }

        public AmoebaWaypoint Waypoint { get; protected set; }

        public override void WireUp()
        {
            Waypoint = GetComponentInParent<AmoebaWaypoint>();
            if (Waypoint) Waypoint.Hatches.Add(this);
        }
    }
}
