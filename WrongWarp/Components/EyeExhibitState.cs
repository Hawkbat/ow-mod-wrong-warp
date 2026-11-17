using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class EyeExhibitState : VisibilityObject
    {
        public QuantumExhibit QuantumExhibit;

        GravityVolume gravityWell;
        QuantumSocket socket;

        public QuantumSocket QuantumSocket => socket;
        public bool IsStateActive() => socket.IsOccupied();

        public override void Awake()
        {
            base.Awake();
            gravityWell = transform.root.Find("GravityWell").GetComponent<GravityVolume>();
            socket = GetComponent<QuantumSocket>();
            if (socket._visibilityObject == this)
            {
                GetSector().OnSectorOccupantsUpdated -= socket.OnSectorOccupantsUpdated;
            }
            SetSector(null);
            CheckEnabled();
            WrongWarpMod.Instance.QuantumExhibit.RegisterExhibitState(this);
        }

        protected void Start()
        {
            socket.enabled = true;
            
            // Quantum exhibit starts as a child, so its visibility trackers get picked up from GetComponentsInChildren
            _visibilityTrackers = _visibilityTrackers.Except(WrongWarpMod.Instance.QuantumExhibit.GetQuantumExhibit()._visibilityTrackers).ToArray();
        }

        public override void Update()
        {
            base.Update();
            if (gravityWell != null && gravityWell.IsVolumeActive() != IsStateActive())
            {
                gravityWell.SetVolumeActivation(IsStateActive());
            }
        }
    }
}
