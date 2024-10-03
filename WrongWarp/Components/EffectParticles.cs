using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class EffectParticles : WrongWarpBehaviour
    {
        public float MinBurstDelay = 1f;
        public float MaxBurstDelay = 1f;
        public int MinBurstCount = 1;
        public int MaxBurstCount = 1;
        public float BurstChance = 1f;
        public OWLight2 BurstLight;
        public float BurstLightDuration = 0.05f;
        public OWAudioSource BurstSound;
        public OWAudioSource LoopSound;
        public float LoopSoundFadeDuration;
        public bool RandomizeRotationY;
        public List<Sensor> SpawnSensors = [];

        [SerializeField]
        bool spawningEnabled = false;

        float lastBurstTime = 0f;
        float burstTimer = 0f;

        ParticleSystem ps;

        public bool SpawningEnabled
        {
            get => spawningEnabled;
            set
            {
                if (spawningEnabled != value)
                {
                    spawningEnabled = value;
                    OnSpawnEnableChanged();
                } else
                {
                    spawningEnabled = value;
                }
            }
        }

        public override void WireUp()
        {
            ps = GetComponentInChildren<ParticleSystem>();
            var emission = ps.emission;
            emission.enabled = false;
            OnSpawnEnableChanged();
        }

        protected void Update()
        {
            if (SpawnSensors.Any())
            {
                SpawningEnabled = Sensor.AreAllActivated(SpawnSensors);
            }
            burstTimer -= Time.deltaTime;
            if (spawningEnabled && burstTimer <= 0f)
            {
                burstTimer = UnityEngine.Random.Range(MinBurstDelay, MaxBurstDelay);
                if (UnityEngine.Random.value <= BurstChance)
                {
                    lastBurstTime = Time.time;
                    if (RandomizeRotationY)
                    {
                        transform.localEulerAngles += Vector3.up * UnityEngine.Random.Range(-180f, 180f);
                    }
                    if (ps)
                    {
                        ps.Emit(UnityEngine.Random.Range(MinBurstCount, MaxBurstCount + 1));
                    }
                    if (BurstSound)
                    {
                        BurstSound.Play();
                    }
                }
            }
            if (BurstLight)
            {
                BurstLight.SetActivation(spawningEnabled && Time.time < lastBurstTime + BurstLightDuration);
            }
        }

        void OnSpawnEnableChanged()
        {
            burstTimer = 0f;
            if (LoopSound)
            {
                LoopSound.FadeTo(spawningEnabled ? 1f : 0f, LoopSoundFadeDuration);
            }
        }
    }
}
