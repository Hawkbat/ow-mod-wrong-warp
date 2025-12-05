using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public abstract class AmoebaGunMode : MonoBehaviour
    {
        protected AmoebaGunItem.AmmoType ammoType;
        protected AmoebaGunItem gun;
        protected bool isFiring;

        protected virtual void Awake()
        {
            gun = GetComponentInParent<AmoebaGunItem>();
            ammoType = GetComponent<AmoebaGunCanister>().AmmoType;
            enabled = false;
        }

        public AmoebaGunItem.AmmoType AmmoType => ammoType;

        public void SetFiring(bool firing)
        {
            if (isFiring == firing) return;
            isFiring = firing;
            if (isFiring)
            {
                StartFiring();
            }
            else
            {
                StopFiring();
            }
        }

        protected virtual void StartFiring()
        {
            enabled = true;
        }

        protected virtual void StopFiring()
        {
            enabled = false;
        }

        protected virtual void UpdateFiring()
        {
            
        }

        protected void LateUpdate()
        {
            if (!gun) return;
            if (isFiring)
            {
                UpdateFiring();
            }
        }
    }
}
