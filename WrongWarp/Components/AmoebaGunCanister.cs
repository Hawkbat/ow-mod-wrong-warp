using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class AmoebaGunCanister : WrongWarpBehaviour
    {
        public AmoebaGunItem.AmmoType AmmoType;
        [SerializeField] ParticleSystem particles;
        [SerializeField] bool unlocked;
        [SerializeField] float amount;
        [SerializeField] GameObject lockedObj;

        AmoebaGunMode firingMode;
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[100];

        protected void Awake()
        {
            firingMode = GetComponent<AmoebaGunMode>();
        }

        public override void WireUp()
        {
            OnAmmoChanged();
        }

        public bool IsUnlocked() => unlocked;
        public float GetAmount() => amount;

        public AmoebaGunMode GetFiringMode() => firingMode;

        public bool Lock()
        {
            if (!unlocked) return false;
            unlocked = false;
            OnAmmoChanged();
            return true;
        }

        public bool Unlock()
        {
            if (unlocked) return false;
            unlocked = true;
            OnAmmoChanged();
            Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.ToolItemWarpCoreInsert);
            return true;
        }

        public float RestoreAmount(float value)
        {
            if (amount >= 1f) return 0f;
            var delta = Mathf.Min(1f - amount, value);
            amount += delta;
            OnAmmoChanged();
            return delta;
        }

        public float ConsumeAmount(float value) {
            if (amount <= 0f) return 0f;
            var delta = Mathf.Min(amount, value);
            amount -= delta;
            OnAmmoChanged();
            return delta;
        }

        void OnAmmoChanged()
        {
            gameObject.SetActive(unlocked);
            if (lockedObj) lockedObj.SetActive(!unlocked);
            
            var max = Mathf.RoundToInt(particles.main.maxParticles * amount);
            if (particles.particleCount < max)
            {
                particles.Emit(max - particles.particleCount);
            } else if (particles.particleCount > max)
            {
                var count = particles.GetParticles(particleArray);
                for (int i = max; i < count; i++)
                {
                    particleArray[i].remainingLifetime = 0f;
                }
                particles.SetParticles(particleArray, count);
            }
        }
    }
}
