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
        DarkZone darkZone;
        GravityVolume gravityWell;
        Light ambientLight;

        public QuantumSocket QuantumSocket => socket;

        void OnEnable()
        {
            socket = GetComponentInChildren<QuantumSocket>();
            darkZone = GetComponentInChildren<DarkZone>();
            gravityWell = transform.root.Find("GravityWell").GetComponent<GravityVolume>();
            ambientLight = transform.root.Find("Sector/AmbientLight").GetComponent<Light>();
            darkZone._ambientLight = ambientLight;
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
            bool active = socket.IsOccupied();
            if (gravityWell != null && gravityWell.IsVolumeActive() != active)
            {
                gravityWell.SetVolumeActivation(active);
            }
        }
    }
}
