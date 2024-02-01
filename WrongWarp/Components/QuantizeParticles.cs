using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    [ExecuteInEditMode]
    public class QuantizeParticles : MonoBehaviour
    {
        public float GridSize = 1f;

        ParticleSystem ps;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[2048];

        void Start()
        {

        }

        void LateUpdate()
        {
            GridSize = Mathf.Max(0.0001f, GridSize);
            if (!ps) ps = GetComponent<ParticleSystem>();
            if (!ps) return;
            var count = ps.GetParticles(particles);
            for (int i = 0; i < count; i++)
            {
                var p = particles[i].position;
                p.x = Mathf.Round(p.x / GridSize) * GridSize;
                p.y = Mathf.Round(p.y / GridSize) * GridSize;
                p.z = Mathf.Round(p.z / GridSize) * GridSize;
                particles[i].position = p;
            }
            ps.SetParticles(particles, count);
        }
    }
}
