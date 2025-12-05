using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class AmoebaStream : MonoBehaviour
    {
        [SerializeField] ParticleSystem stream;
        [SerializeField] float streamSpread = 0.5f;
        [SerializeField] bool reverseStream;
        [SerializeField] ParticleSystem start;
        [SerializeField] ParticleSystem end;
        [SerializeField] Transform startTarget;
        [SerializeField] Transform endTarget;

        ParticleSystem.Particle[] particles;
        float fadeTimer;

        protected void Awake()
        {
            particles = new ParticleSystem.Particle[stream.main.maxParticles];
        }

        public void SetPoints(Transform startTarget, Transform endTarget)
        {
            this.startTarget = startTarget;
            this.endTarget = endTarget;

            UpdateParticles();
        }

        public void SetEnabled(bool enabled)
        {
            var emission = stream.emission;
            emission.enabled = enabled;

            if (start)
            {
                var startEmission = start.emission;
                startEmission.enabled = enabled;
            }

            if (end)
            {
                var endEmission = end.emission;
                endEmission.enabled = enabled;
            }

            if (!this.enabled)
            {
                this.enabled = true;
            }
            else
            {
                fadeTimer = 1f;
            }

            UpdateParticles();
        }

        protected void LateUpdate()
        {
            UpdateParticles();

            if (fadeTimer > 0f)
            {
                fadeTimer -= Time.deltaTime;
                if (fadeTimer <= 0f)
                {
                    enabled = false;
                }
            }
        }

        void UpdateParticles()
        {
            var startPos = startTarget ? startTarget.position : transform.position;
            var endPos = endTarget ? endTarget.position : transform.position;

            // Update particle positions to create the stream effect
            var particleCount = stream.GetParticles(particles);
            for (int i = 0; i < particleCount; i++)
            {
                float t = 1f - (particles[i].remainingLifetime / particles[i].startLifetime);
                if (reverseStream) t = 1f - t;
                particles[i].position = Vector3.Lerp(startPos, endPos, t);
                // Particles spread out perpendicular to the travel line over time, peaking at the middle before converging back to the end point
                // Angle of spread is psuedo-random based on particle index
                float spreadFactor = 1f - 4f * (t - 0.5f) * (t - 0.5f); // Parabola peaking at t=0.5
                float angle = (i * 137.508f) * Mathf.Deg2Rad; // Use golden angle for pseudo-random distribution
                Vector3 direction = (endPos - startPos).normalized;
                Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
                Vector3 spreadOffset = perpendicular * Mathf.Sin(angle) * streamSpread * spreadFactor;
                particles[i].position += spreadOffset;
            }
            stream.SetParticles(particles, particleCount);

            // Position the start and end particle systems at the respective points
            if (start)
            {
                start.transform.position = startPos;
            }
            if (end)
            {
                end.transform.position = endPos;
            }
        }
    }
}
