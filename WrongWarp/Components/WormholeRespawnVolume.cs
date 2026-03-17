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
                if (!WrongWarpMod.Instance.SaveData[SaveDataFlag.HasPlayedMuseumMelody])
                {
                    // Black hole is still a white hole, do nothing
                    return;
                }
                if (WrongWarpMod.Instance.SaveData[SaveDataFlag.RespawnDisabled])
                {
                    var hasSeed = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItemType() == CuratorSeedItem.ItemType;
                    WrongWarpMod.Instance.SaveData[SaveDataFlag.BroughtCoreToEye] = hasSeed;
                    WrongWarpMod.Instance.Warp.WarpToEye();
                    return;
                }

                DialogueConditionManager.SharedInstance.SetConditionState("WW_REACT_WORMHOLE_RESPAWN", true);
                WrongWarpMod.Instance.Respawner.RespawnPlayer();
                WrongWarpMod.Instance.Respawner.RespawnShip();
            }
        }

        public override void OnEffectVolumeExit(GameObject hitObj)
        {

        }
    }
}
