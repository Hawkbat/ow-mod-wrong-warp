using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class AmoebaGunStreamMode : AmoebaGunMode
    {
        [SerializeField] float ammoConsumptionRate = 0.2f;
        [SerializeField] AmoebaStream stream;
        [SerializeField] OWAudioSource startFiringAudio;
        [SerializeField] OWAudioSource stopFiringAudio;
        [SerializeField] OWAudioSource firingAudio;

        Transform end;

        protected override void Awake()
        {
            base.Awake();
            end = new GameObject("AmoebaGunStreamEndpoint").transform;
        }

        protected override void StartFiring()
        {
            base.StartFiring();
            stream.SetEnabled(true);
            stream.SetPoints(gun.GetMuzzleTransform(), end);
            if (startFiringAudio)
            {
                startFiringAudio.Play();
            }
            if (firingAudio)
            {
                firingAudio.Play();
            }
        }

        protected override void StopFiring()
        {
            base.StopFiring();
            stream.SetEnabled(false);
            if (stopFiringAudio)
            {
                stopFiringAudio.Play();
            }
            if (firingAudio)
            {
                firingAudio.Stop();
            }
        }

        protected override void UpdateFiring()
        {
            base.UpdateFiring();
            var ammoSpent = gun.ConsumeAmmo(AmmoType, ammoConsumptionRate * Time.deltaTime);

            var cam = Locator.GetPlayerCamera();
            var ray = new Ray(cam.transform.position, cam.transform.forward);

            Collider hitCollider;

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, OWLayerMask.blockableInteractMask, QueryTriggerInteraction.Ignore))
            {
                end.parent = hitInfo.transform;
                end.position = hitInfo.point;
                hitCollider = hitInfo.collider;
            }
            else
            {
                end.parent = Locator.GetPlayerTransform();
                end.position = ray.origin + ray.direction * 1000f;
                hitCollider = null;
            }


            if (hitCollider)
            {
                var target = hitCollider.GetComponentInParent<IAmoebaGunTarget>();
                if (target != null)
                {
                    target.OnAmoebaGunHit(gun, AmmoType, ammoSpent, end.position);
                }
            }
        }
    }
}
