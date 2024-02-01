using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{

    public class WormholeRespawnVolume : EffectVolume
    {
        public override void OnEffectVolumeEnter(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                if (WrongWarpMod.Instance.SaveData.HasPlayedMuseumMelody && !WrongWarpMod.Instance.SaveData.RespawnDisabled)
                {
                    var notification = new NotificationData(NotificationTarget.All, $"EMERGENCY RESPAWN ACTIVATED");
                    NotificationManager.SharedInstance.PostNotification(notification);
                    WrongWarpMod.Instance.Respawner.RespawnPlayer();
                    WrongWarpMod.Instance.Respawner.RespawnShip();
                }
            }
        }

        public override void OnEffectVolumeExit(GameObject hitObj)
        {

        }
    }
}
