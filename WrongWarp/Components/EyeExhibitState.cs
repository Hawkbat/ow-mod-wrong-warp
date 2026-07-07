using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class EyeExhibitState : WrongWarpBehaviour
    {
        public QuantumExhibit QuantumExhibit;

        QuantumSocket socket;
        GravityVolume gravityWell;

        public QuantumSocket QuantumSocket => socket;
        public bool IsStateActive() => socket.IsOccupied();

        void OnEnable()
        {
            socket = GetComponentInChildren<QuantumSocket>();
            gravityWell = transform.root.Find("GravityWell").GetComponent<GravityVolume>();
        }

        void OnDisable()
        {

        }

        public override void WireUp()
        {
            Mod.QuantumExhibit.RegisterExhibitState(this);
        }

        void Update()
        {
            if (gravityWell != null && gravityWell.IsVolumeActive() != IsStateActive())
            {
                gravityWell.SetVolumeActivation(IsStateActive());
            }
        }
    }
}
