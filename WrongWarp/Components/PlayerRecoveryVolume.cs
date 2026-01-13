using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class PlayerRecoveryVolume : EffectVolume
    {
        [SerializeField] bool refuelsPlayer = true;
        [SerializeField] bool healsPlayer = true;
        [SerializeField] bool cleansVisor;
        [SerializeField] bool DLCFuelTank;

        bool isRefilling = false;

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (isRefilling)
            {
                isRefilling = false;
                var res = Locator.GetPlayerTransform().GetComponent<PlayerResources>();
                res.StopRefillResources();
            }
        }

        public override void OnEffectVolumeEnter(GameObject hitObj)
        {
            if (!hitObj.CompareTag("PlayerDetector"))
            {
                return;
            }
            isRefilling = true;
            var res = Locator.GetPlayerTransform().GetComponent<PlayerResources>();
            res.StartRefillResources(refuelsPlayer, healsPlayer, DLCFuelTank);
            var visor = Locator.GetPlayerCamera().GetComponentInChildren<VisorEffectController>();
            if (cleansVisor)
            {
                visor.ClearVisorDirt();
            }
            var audio = Locator.GetPlayerAudioController();
            if (refuelsPlayer)
            {
                audio.PlayRefuel();
            }
            if (healsPlayer)
            {
                audio.PlayMedkit();
            }
        }

        public override void OnEffectVolumeExit(GameObject hitObj)
        {
            if (!hitObj.CompareTag("PlayerDetector"))
            {
                return;
            }

            isRefilling = false;
            var res = Locator.GetPlayerTransform().GetComponent<PlayerResources>();
            res.StopRefillResources();
        }
    }
}
