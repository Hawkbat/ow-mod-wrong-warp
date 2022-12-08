using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class AmoebaWaypoint : WrongWarpBehaviour
    {
        public List<AmoebaHatch> Hatches { get; protected set; } = new List<AmoebaHatch>();
        public AmoebaCircuit Circuit { get; protected set; }

        public override void WireUp()
        {
            Circuit = GetComponentInParent<AmoebaCircuit>();
            if (Circuit) Circuit.Waypoints.Add(this);
        }

        public Vector3 GetBezierPosition(Vector3 startPosition, Quaternion startRotation, Vector3 endPosition, Quaternion endRotation, float t)
        {
            Vector3 p0 = startPosition;
            Vector3 p1 = p0 + startRotation * Vector3.forward;
            Vector3 p3 = endPosition;
            Vector3 p2 = endPosition + -(endRotation * Vector3.forward);
            return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
        }
    }
}
