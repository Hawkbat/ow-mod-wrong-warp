using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class PrefabSector : MonoBehaviour
    {
        [SerializeField] bool fixcolliderMasks;

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

            // Fix colliders not being enabled by collision group due to ancient prefabs having zeroed masks
            if (fixcolliderMasks)
            {
                foreach (var col in collisionGroup._colliders)
                {
                    col.SetLODActivationMask(DynamicOccupant.Player | DynamicOccupant.Probe | DynamicOccupant.Ship);
                }
            }
            else
            {
                // Check for zeroed masks and log a warning if found
                foreach (var col in collisionGroup._colliders)
                {
                    if (col != null && (col._lodActivationMask == null || col._lodActivationMask._mask == 0))
                    {
                        Debug.LogWarning($"Collider {UnityUtils.GetTransformPath(col.transform)} in sector {name} has a zeroed LOD activation mask. This may cause it to not activate properly. Fix the OWCollider properties or enable the 'Fix Collider Masks' option on this component.", col);
                    }
                }
            }
        }
    }
}
