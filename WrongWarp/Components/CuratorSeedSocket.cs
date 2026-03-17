using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class CuratorSeedSocket : OWItemSocket
    {
        [SerializeField] AudioVolume musicVolume;
        [SerializeField] OWAudioSource unsocketAudio;
        
        Light planetAmbientLight;
        float originalAmbientIntensity;

        public override bool UsesGiveTakePrompts() => true;

        public override void Awake()
        {
            base.Awake();
            _acceptableType = CuratorSeedItem.ItemType;
            planetAmbientLight = transform.root.Find("Sector/AmbientLight").GetComponent<Light>();
            originalAmbientIntensity = planetAmbientLight.intensity;
            WrongWarpMod.Instance.SaveData[SaveDataFlag.RespawnDisabled] = false;
        }

        public override bool PlaceIntoSocket(OWItem item)
        {
            if (base.PlaceIntoSocket(item))
            {
                WrongWarpMod.Instance.SaveData[SaveDataFlag.RespawnDisabled] = false;
                TimeLoop.SetTimeLoopEnabled(true);
                musicVolume.GetOWTriggerVolume().SetTriggerActivation(true);
                musicVolume.Activate();
                planetAmbientLight.intensity = originalAmbientIntensity;
                unsocketAudio.pitch = -1f;
                unsocketAudio.Play();
                return true;
            }
            return false;
        }

        public override OWItem RemoveFromSocket()
        {
            WrongWarpMod.Instance.SaveData[SaveDataFlag.RespawnDisabled] = true;
            TimeLoop.SetTimeLoopEnabled(false);
            musicVolume.GetOWTriggerVolume().SetTriggerActivation(false);
            musicVolume.Deactivate();
            planetAmbientLight.intensity = 0.2f;
            unsocketAudio.pitch = 1f;
            unsocketAudio.Play();
            return base.RemoveFromSocket();
        }
    }
}
