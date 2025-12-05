using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class AmoebaGunProjectileMode : AmoebaGunMode
    {
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] OWAudioSource fireAudio;
        [SerializeField] float ammoPerShot = 0.2f;
        [SerializeField] float fireRate = 0.5f;
        [SerializeField] float launchForce = 500f;

        float fireCooldown;

        protected override void StartFiring()
        {
            base.StartFiring();
        }

        protected override void StopFiring()
        {
            base.StopFiring();
        }

        protected override void UpdateFiring()
        {
            base.UpdateFiring();
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                var ammoSpent = gun.ConsumeAmmo(AmmoType, ammoPerShot);
                if (ammoSpent > 0f)
                {
                    fireCooldown = fireRate;
                    var spawnPosition = gun.GetMuzzleTransform().position;
                    var spawnRotation = gun.GetMuzzleTransform().rotation;
                    var obj = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
                    var projectile = obj.GetComponent<AmoebaGunProjectile>();
                    if (projectile != null)
                    {
                        projectile.Initialize(gun, AmmoType, ammoSpent);
                    }
                    var rb = obj.GetComponent<OWRigidbody>();
                    if (rb)
                    {
                        rb.AddForce(gun.GetMuzzleTransform().forward * launchForce);
                    }
                    if (fireAudio)
                    {
                        fireAudio.Play();
                    }
                }
            }
        }
    }
}
