using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class AmoebaSwarm : WrongWarpBehaviour
    {
        public float Speed;
        public float Lifetime;

        public AmoebaCircuit Circuit;
        public AmoebaHatch NextHatch;

        private bool inWaypoint = false;
        private float t = 0f;
        private Bezier curve = new();
        private float curveLength = 0f;
        private float killTimer = 0f;

        private ParticleSystem ps;
        private ParticleSystem.Particle[] particles;

        public override void WireUp()
        {

        }

        public void SetUp(AmoebaWaypoint waypoint)
        {
            inWaypoint = true;
            var nextHatch = waypoint.Hatches.FirstOrDefault(h => !h.IsBlocked);
            if (nextHatch)
            {
                NextHatch = nextHatch;
                UpdateTarget(NextHatch.transform);
            }
        }

        public void Update()
        {
            var beforePos = transform.localPosition;

            t = Mathf.Clamp01(t + Speed * Time.deltaTime / curveLength);
            transform.localPosition = curve.GetPosition(t);
            transform.localRotation = Quaternion.LookRotation(curve.GetDirection(t, Vector3.up), Vector3.up);

            var afterPos = transform.localPosition;

            var deltaPos = (afterPos - beforePos);

            if (!ps)
            {
                ps = GetComponentInChildren<ParticleSystem>();
                particles = new ParticleSystem.Particle[ps.main.maxParticles];
                ps.transform.parent = transform.parent;
                ps.transform.localPosition = Vector3.zero;

                var particleCount = ps.GetParticles(particles);
                for (int i = 0; i < particleCount; i++)
                    particles[i].position += transform.localPosition + deltaPos;
                ps.SetParticles(particles);
            } else
            {
                var particleCount = ps.GetParticles(particles);
                for (int i = 0; i < particleCount; i++)
                    particles[i].position += deltaPos;
                ps.SetParticles(particles);
            }

            if (t == 1f || (NextHatch && NextHatch.IsBlocked))
            {
                if (NextHatch)
                {
                    if (t == 1f && !inWaypoint && NextHatch.Waypoint == NextHatch.Waypoint.Circuit.SpawnWaypoint)
                    {
                        UpdateTarget(NextHatch.Waypoint.transform);
                        NextHatch = null;
                    }
                    else if (inWaypoint)
                    {
                        var nextHatch = NextHatch.Linked && !NextHatch.Linked.IsBlocked ? NextHatch.Linked : null;
                        if (nextHatch)
                        {
                            NextHatch = nextHatch;
                            inWaypoint = false;
                            UpdateTarget(NextHatch.transform);
                        }
                    }
                    else if (!inWaypoint)
                    {
                        var nextHatch = NextHatch.Waypoint.Hatches
                            .OrderByDescending(h => h.SwarmPreference)
                            .FirstOrDefault(h => h != NextHatch && !h.IsBlocked && h.Linked && !h.Linked.IsBlocked);

                        if (nextHatch)
                        {
                            NextHatch = nextHatch;
                            inWaypoint = true;
                            UpdateTarget(NextHatch.transform);
                        }
                    }
                }
            }

            killTimer += Time.deltaTime;
            if (killTimer > Lifetime)
            {
                ps.transform.parent = transform;
                Circuit.Reclaim(this);
            }
        }

        public void UpdateTarget(Transform target)
        {
            t = 0f;
            curve.StartPosition = Circuit.transform.InverseTransformPoint(transform.position);
            curve.StartRotation = Circuit.transform.InverseTransformRotation(transform.rotation);
            curve.EndPosition = Circuit.transform.InverseTransformPoint(target.position);
            curve.EndRotation = Circuit.transform.InverseTransformRotation(target.rotation);
            curve.InvertTangents = !inWaypoint;
            curveLength = curve.GetLength();
        }

        public struct Bezier
        {
            public Vector3 StartPosition;
            public Quaternion StartRotation;
            public Vector3 EndPosition;
            public Quaternion EndRotation;
            public bool InvertTangents;

            public Vector3 GetPosition(float t)
            {
                t = Mathf.Clamp01(t);
                var smoothing = Vector3.Distance(StartPosition, EndPosition) * 0.5f;
                var p0 = StartPosition;
                var p1 = p0 + StartRotation * Vector3.forward * 1f * smoothing;
                var p3 = EndPosition;
                var p2 = EndPosition - EndRotation * -Vector3.forward * (InvertTangents ? -1f : 1f) * smoothing;
                return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
            }

            public Vector3 GetDirection(float t, Vector3 up)
            {
                var p0 = GetPosition(t - 0.0001f);
                var p1 = GetPosition(t + 0.0001f);
                var dir = (p1 - p0).normalized;
                return Quaternion.LookRotation(dir, up) * Vector3.forward;
            }

            public float GetLength()
            {
                var length = 0f;
                var steps = 30;
                for (int i = 0; i < steps; i++)
                {
                    var t0 = Mathf.InverseLerp(0, steps, i);
                    var t1 = Mathf.InverseLerp(0, steps, i + 1);
                    var p0 = GetPosition(t0);
                    var p1 = GetPosition(t1);
                    length += Vector3.Distance(p0, p1);
                }
                if (length == 0f) length = 0.000001f;
                return length;
            }

            public Vector3 GetClosestPoint(Vector3 target)
            {
                var resultDist = float.PositiveInfinity;
                Vector3 resultPoint = target;

                var steps = 30;
                for (int i = 0; i <= steps; i++)
                {
                    var t = Mathf.InverseLerp(0, steps, i);
                    var p = GetPosition(t);
                    var dist = Vector3.Distance(target, p);
                    if (dist < resultDist)
                    {
                        resultDist = dist;
                        resultPoint = p;
                    }
                }
                return resultPoint;
            }
        }
    }
}
