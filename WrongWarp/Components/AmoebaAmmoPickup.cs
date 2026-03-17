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
        [SerializeField] AmoebaGunItem.AmmoType ammoType = default;
        [SerializeField] float ammoAmount = default;
        [SerializeField] bool unlocksAmmoType = default;
        [SerializeField] ParticleSystem[] particlesToDisable = default;
        [SerializeField] GameObject[] objectsToDisable = default;
        [SerializeField] FrequencyAsset pickupMessageAsset = default;

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

            if (!amoebaGun.HasUnlockedAmmo(ammoType))
            {
                if (unlocksAmmoType)
                {
                    amoebaGun.UnlockAmmo(ammoType);
                }
                else
                {
                    return;
                }
            }

            amoebaGun.RestoreAmmo(ammoType, ammoAmount);
            SetVolumeActivation(false);
            foreach (var obj in objectsToDisable)
            {
                obj.SetActive(false);
            }
            foreach (var ps in particlesToDisable)
            {
                ps.Stop();
            }

            var pickupMsg = WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(pickupMessageAsset.FullID);
            NotificationManager.SharedInstance.PostNotification(new NotificationData(NotificationTarget.All, pickupMsg));
        }

        public override void OnEffectVolumeExit(GameObject hitObj)
        {

        }
    }
}
