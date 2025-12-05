using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    [RequireComponent(typeof(OWRigidbody))]
    public class AmoebaGunProjectile : MonoBehaviour
    {
        [SerializeField] DitheringAnimator ditheringAnimator;
        [SerializeField] float lifeTime = 5f;
        [SerializeField] ParticleSystem particles;
        [SerializeField] float destroyDelay = 2f;
        [SerializeField] ImpactType impactType = ImpactType.EndOnImpact;

        AmoebaGunItem gun;
        AmoebaGunItem.AmmoType ammoType;
        float ammoAmount;

        float age = 0f;

        public void Initialize(AmoebaGunItem gun, AmoebaGunItem.AmmoType ammoType, float ammoAmount)
        {
            this.gun = gun;
            this.ammoType = ammoType;
            this.ammoAmount = ammoAmount;
        }

        protected void Update()
        {
            age += Time.deltaTime;
            if (ReachedTime(lifeTime))
            {
                EndLife();
            }
            if (ReachedTime(lifeTime + destroyDelay))
            {
                enabled = false;
                Destroy(gameObject);
            }
        }

        protected void OnCollisionEnter(Collision collision)
        {
            if (age >= lifeTime) return;

            var target = collision.collider.GetComponentInParent<IAmoebaGunTarget>();
            if (target != null)
            {
                var contactPoint = collision.GetContact(0).point;
                target.OnAmoebaGunHit(gun, ammoType, ammoAmount, contactPoint);
                if (impactType == ImpactType.EndOnTargetHit)
                {
                    EndLife();
                }
            }

            if (impactType == ImpactType.EndOnImpact)
            {
                EndLife();
            }
        }

        bool ReachedTime(float time)
        {
            var prevAge = age - Time.deltaTime;
            return age >= time && prevAge < time;
        }

        void EndLife()
        {
            age = lifeTime;
            if (ditheringAnimator)
            {
                ditheringAnimator.SetVisible(false, Mathf.Min(0.5f, destroyDelay));
            }
            var emission = particles.emission;
            emission.enabled = false;
        }

        public enum ImpactType
        {
            EndOnImpact,
            EndOnTargetHit,
            None,
        }
    }
}
