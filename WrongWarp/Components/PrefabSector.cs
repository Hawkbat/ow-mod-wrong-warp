using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class PrefabSector : MonoBehaviour
    {
        Sector sector;
        SectorCollisionGroup collisionGroup;

        protected void Awake()
        {
            sector = GetComponent<Sector>();
            collisionGroup = GetComponent<SectorCollisionGroup>();
        }

        protected void Start()
        {
            // Fix parent sector; not sure if it's actually necessary
            var parentSector = transform.parent.GetComponentInParent<Sector>();
            sector.SetParentSector(parentSector);

            // Fix sector trigger shape being disabled by collision group, preventing sector from ever activating
            var shape = GetComponent<Shape>();
            collisionGroup._shapes.Remove(shape);
            shape.enabled = true;
            enabled = false;

            // Fix colliders not being enabled by collision group due to LODs being uninitialized
            foreach (var col in collisionGroup._colliders)
            {
                col.SetLODActivationMask(DynamicOccupant.Player | DynamicOccupant.Probe | DynamicOccupant.Ship);
            }
        }
    }
}
