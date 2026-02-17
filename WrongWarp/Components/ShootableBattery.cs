using System;
using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp.Components
{
    public class ShootableBattery : Sensor, IAmoebaGunTarget
    {
        public float charge = 0f;
        public float maxCharge = 1f;

        public override float ComputeStrength() => charge / maxCharge;

        public override void WireUp()
        {

        }

        public bool OnAmoebaGunHit(AmoebaGunItem gun, AmoebaGunItem.AmmoType ammoType, float amount, Vector3 point)
        {
            if (ammoType == AmoebaGunItem.AmmoType.Charge)
            {
                var chargedAmount = Mathf.Min(amount, maxCharge - charge);
                charge += chargedAmount;
                return chargedAmount > 0f;
            }
            else if (ammoType == AmoebaGunItem.AmmoType.Drain)
            {
                var drainedAmount = Mathf.Min(amount, charge);
                charge -= drainedAmount;
                gun.RestoreAmmo(AmoebaGunItem.AmmoType.Charge, drainedAmount);
                return drainedAmount > 0f;
            }
            return false;
        }
    }
}
