using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CapsuleCollider), typeof(OWTriggerVolume))]
    public class LightElevator : FluidVolume
    {
        [SerializeField] float length = 1f;
        [SerializeField] float radius = 1f;
        [SerializeField] float ringDensity = 10f;
        [SerializeField] float sparkleDensity = 25f;
        [SerializeField] float idleSpeed = 0.1f;
        [SerializeField] float maxPlayerSpeed = 10f;
        [SerializeField] float verticalForce = 5f;
        [SerializeField] float inwardForce = 15f;
        [SerializeField] float endZoneSize = 4f;
        [SerializeField] ParticleSystem rings;
        [SerializeField] ParticleSystem sparkles;
        [SerializeField] DirectionalForceVolume alignmentVolume;
        [SerializeField] Transform topEmitter;
        [SerializeField] Transform bottomEmitter;

        float input;
        CapsuleCollider fieldShape;
        bool playerInside;

        ParticleSystem.Particle[] particles;
        float previousLength;
        float previousRadius;
        float previousSpeed;

        public override void Awake()
        {
            base.Awake();
            fieldShape = GetComponent<CapsuleCollider>();
        }

        public override void Start()
        {
            base.Start();
            UpdateParameters(true);
            if (!Application.isEditor)
            {
                enabled = false;
            }
        }

        protected void Update()
        {
            if (playerInside)
            {
                var thrustInput = Mathf.Clamp(OWInput.GetValue(InputLibrary.thrustUp) + OWInput.GetValue(InputLibrary.boost) + OWInput.GetValue(InputLibrary.jump) - OWInput.GetValue(InputLibrary.thrustDown), -1f, 1f);
                input = thrustInput;
            }
            UpdateParameters();
        }

        public override void OnEffectVolumeEnter(GameObject hitObj)
        {
            base.OnEffectVolumeEnter(hitObj);

            if (hitObj.CompareTag("PlayerDetector"))
            {
                playerInside = true;
                enabled = true;

                var localPosition = transform.InverseTransformPoint(hitObj.transform.position);
                var heightFactor = Mathf.Clamp01(Mathf.InverseLerp(0f, length, localPosition.y));
                // Move the player to the other end
                input = Mathf.Lerp(1f, -1f, heightFactor);
            }
        }

        public override void OnEffectVolumeExit(GameObject hitObj)
        {
            base.OnEffectVolumeExit(hitObj);

            if (hitObj.CompareTag("PlayerDetector"))
            {
                playerInside = false;
                UpdateParameters();
                enabled = false;
            }
        }

        public override Vector3 GetPointFluidVelocity(Vector3 worldPosition, FluidDetector detector)
        {
            var result = Vector3.zero;

            var ownVelocity = _attachedBody.GetPointVelocity(worldPosition);
            result += ownVelocity;

            result += transform.up * input * verticalForce;

            var relativePos = worldPosition - transform.position;
            var parallelOffset = Vector3.Project(relativePos, transform.up);
            var perpendicularOffset = relativePos - parallelOffset;
            var distFromCenter = perpendicularOffset.magnitude;
            var distFromBottom = parallelOffset.magnitude;

            if (distFromBottom > endZoneSize && distFromBottom < length - endZoneSize)
            {
                var relativeVelocity = _attachedBody.GetRelativeVelocity(detector.GetAttachedOWRigidbody());
                var perpendicularVelocity = Vector3.Project(relativeVelocity, perpendicularOffset.normalized);

                if (Vector3.Dot(perpendicularVelocity, perpendicularOffset) > 0f)
                {
                    var radiusFactor = Mathf.Clamp01(distFromCenter / radius);
                    var perpForce = -perpendicularVelocity * radiusFactor;
                    result += perpForce;
                }
                else if (Math.Abs(input) > 0.1f)
                {
                    var perpForce = -perpendicularOffset.normalized * inwardForce;
                    result += perpForce;
                }

            }

            return result;
        }

        void UpdateParameters(bool force = false)
        {
            var speed = playerInside ? input * maxPlayerSpeed : idleSpeed;
            if (speed == 0f) speed = 0.0001f;

            var lengthChanged = length != previousLength;
            previousLength = length;
            var radiusChanged = radius != previousRadius;
            previousRadius = radius;
            var speedChanged = speed != previousSpeed;
            previousSpeed = speed;
            var parametersChanged = lengthChanged || radiusChanged || speedChanged;

            if (!parametersChanged) return;

            var start = transform.position;
            var end = transform.position + transform.up * length;

            var lifetime = length / Mathf.Abs(speed);

            var ringEmissionRate = ringDensity / lifetime;
            var ringMaxParticles = Mathf.CeilToInt(ringEmissionRate * lifetime);

            var sparkleEmissionRate = sparkleDensity / lifetime;
            var sparkleMaxParticles = Mathf.CeilToInt(sparkleEmissionRate * lifetime);

            var maxParticles = Mathf.Max(ringMaxParticles, sparkleMaxParticles);

            if (particles == null || particles.Length < maxParticles)
            {
                particles = new ParticleSystem.Particle[maxParticles];
            }

            if (rings)
            {
                rings.transform.position = speed >= 0f ? start : end;

                var main = rings.main;
                main.prewarm = true;
                main.startLifetime = lifetime;
                main.startSpeed = speed;
                main.startSizeX = main.startSizeY = radius * 2f;

                var emission = rings.emission;
                emission.rateOverTime = ringEmissionRate;

                var particleCount = rings.GetParticles(particles);
                for (int i = 0; i < particleCount; i++)
                {
                    var particle = particles[i];
                    var distanceRemaining = Mathf.Abs(particle.position.y - (speed >= 0f ? length : 0f));
                    var lifetimeRemaining = distanceRemaining / Mathf.Abs(speed);
                    particle.startLifetime = lifetime;
                    particle.remainingLifetime = lifetimeRemaining;
                    particle.velocity = Vector3.up * speed;
                    particles[i] = particle;
                }
                rings.SetParticles(particles, particleCount);

                if (force)
                {
                    rings.Stop();
                    rings.Clear();
                    rings.Play();
                }
            }

            if (sparkles)
            {
                sparkles.transform.position = speed >= 0f ? start : end;

                var main = sparkles.main;
                main.prewarm = true;
                main.startLifetime = lifetime;
                main.startSpeed = speed;

                var emission = sparkles.emission;
                emission.rateOverTime = sparkleEmissionRate;

                var particleCount = sparkles.GetParticles(particles);
                for (int i = 0; i < particleCount; i++)
                {
                    var particle = particles[i];
                    var distanceRemaining = Mathf.Abs(particle.position.y - (speed >= 0f ? length : 0f));
                    var lifetimeRemaining = distanceRemaining / Mathf.Abs(speed);
                    particle.startLifetime = lifetime;
                    particle.remainingLifetime = lifetimeRemaining;
                    particle.velocity = Vector3.up * speed;
                    particles[i] = particle;
                }

                if (force)
                {
                    sparkles.Stop();
                    sparkles.Clear();
                    sparkles.Play();
                }
            }

            if (fieldShape)
            {
                fieldShape.center = transform.InverseTransformPoint(Vector3.Lerp(start, end, 0.5f));
                fieldShape.height = length;
                fieldShape.radius = radius;
            }

            if (alignmentVolume)
            {
                alignmentVolume.SetLocalForceDirection(Vector3.down);
                alignmentVolume.SetFieldMagnitude(0.001f);
            }

            if (topEmitter)
            {
                topEmitter.localScale = new Vector3(radius, radius, topEmitter.localScale.z);
                topEmitter.localPosition = Vector3.up * length;
            }

            if (bottomEmitter)
            {
                bottomEmitter.localScale = new Vector3(radius, radius, bottomEmitter.localScale.z);
                bottomEmitter.localPosition = Vector3.zero;
            }
        }
    }
}
