using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public interface IAmoebaGunTarget
    {
        bool OnAmoebaGunHit(AmoebaGunItem gun, AmoebaGunItem.AmmoType ammoType, float amount, Vector3 point);
    }
}
