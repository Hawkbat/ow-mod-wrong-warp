using ModDataTools.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class AmoebaAmmoPickupVolume : EffectVolume
    {
        public AmoebaGunItem.AmmoType AmmoType;
        public float AmmoAmount;
        public bool UnlocksAmmoType;
        public ParticleSystem ParticlesToDisable;
        public FrequencyAsset PickupMessageAsset;

        public override void OnEffectVolumeEnter(GameObject hitObj)
        {
            if (!hitObj.CompareTag("Player"))
            {
                return;
            }
            var amoebaGun = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem() as AmoebaGunItem;
            if (!amoebaGun)
            {
                return;
            }

            if (!amoebaGun.HasUnlockedAmmo(AmmoType))
            {
                if (UnlocksAmmoType)
                {
                    amoebaGun.UnlockAmmo(AmmoType);
                }
                else
                {
                    return;
                }
            }

            amoebaGun.RestoreAmmo(AmmoType, AmmoAmount);
            SetVolumeActivation(false);
            if (ParticlesToDisable != null)
            {
                ParticlesToDisable.Stop();
            }

            var pickupMsg = WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(PickupMessageAsset.FullID);
            NotificationManager.SharedInstance.PostNotification(new NotificationData(NotificationTarget.All, pickupMsg));
        }

        public override void OnEffectVolumeExit(GameObject hitObj)
        {

        }
    }
}
