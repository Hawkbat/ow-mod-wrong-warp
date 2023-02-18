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

        void OnDrawGizmos()
        {
            var hatches = GetComponentsInChildren<AmoebaHatch>();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);
            foreach (var hatch in hatches)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, hatch.transform.position);
                if (hatch.Linked)
                {
                    int steps = 20;
                    for (int i = 0; i < steps; i += 2)
                    {
                        Vector3 start = GetBezierPosition(hatch.transform, hatch.Linked.transform, true, 1f / steps * (i + 0));
                        Vector3 end = GetBezierPosition(hatch.transform, hatch.Linked.transform, true, 1f / steps * (i + 1));
                        Gizmos.DrawLine(start, end);
                    }
                }
                Gizmos.color = Color.green;
                foreach (var other in hatches)
                {
                    if (hatch == other) continue;
                    int steps = 20;
                    for (int i = 0; i < steps; i += 2)
                    {
                        Vector3 start = GetBezierPosition(hatch.transform, other.transform, false, 1f / steps * (i + 0));
                        Vector3 end = GetBezierPosition(hatch.transform, other.transform, false, 1f / steps * (i + 1));
                        Gizmos.DrawLine(start, end);
                    }
                }
            }
        }
        public Vector3 GetBezierPosition(Transform start, Transform end, bool invert, float t)
        {
            return GetBezierPosition(start.position, start.rotation, end.position, end.rotation, invert, t);
        }
        public Vector3 GetBezierPosition(Vector3 startPosition, Quaternion startRotation, Vector3 endPosition, Quaternion endRotation, bool invert, float t)
        {
            float smoothing = Vector3.Distance(startPosition, endPosition) * 0.5f;
            Vector3 p0 = startPosition;
            Vector3 p1 = p0 + startRotation * Vector3.forward * (invert ? -1f : 1f) * smoothing;
            Vector3 p3 = endPosition;
            Vector3 p2 = endPosition - endRotation * -Vector3.forward * (invert ? -1f : 1f) * smoothing;
            return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
        }
    }
}
