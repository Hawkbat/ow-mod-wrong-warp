using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class CuratorCoreInteriorVolume : MonoBehaviour
    {
        [SerializeField] AudioVolume musicVolume;

        OWTriggerVolume trigger;
        TessellatedSphereRenderer waterRenderer;

        protected void Awake()
        {
            trigger = GetComponent<OWTriggerVolume>();
            trigger.OnEntry += OnEntry;
            trigger.OnExit += OnExit;

            waterRenderer = transform.root.Find("Sector/Water").GetComponent<TessellatedSphereRenderer>();
        }

        protected void OnDestroy()
        {
            trigger.OnEntry -= OnEntry;
            trigger.OnExit -= OnExit;
        }

        void OnEntry(GameObject other)
        {
            if (other.CompareTag("PlayerDetector"))
            {
                musicVolume.GetOWTriggerVolume().AddObjectToVolume(Locator.GetPlayerCameraDetector());
                waterRenderer.enabled = false;
            }
        }

        void OnExit(GameObject other)
        {
            if (other.CompareTag("PlayerDetector"))
            {
                musicVolume.GetOWTriggerVolume().RemoveObjectFromVolume(Locator.GetPlayerCameraDetector());
                if (!TimeLoop.IsTimeLoopEnabled())
                {
                    var gmc = Locator.GetGlobalMusicController();
                    FixMusicVolume(gmc._finalEndTimesIntroSource);
                    FixMusicVolume(gmc._finalEndTimesLoopSource);
                    FixMusicVolume(gmc._finalEndTimesDarkBrambleSource);
                    gmc.OnExitTimeLoopCentral(Locator.GetPlayerBody());
                }
                waterRenderer.enabled = true;
            }
        }

        void FixMusicVolume(OWAudioSource audioSrc)
        {
            var musicVolume = 0.7f;
            audioSrc._audioSource.volume = musicVolume;
            audioSrc._maxSourceVolume = musicVolume;
            audioSrc.UpdateSourceVolume();
        }
    }
}
