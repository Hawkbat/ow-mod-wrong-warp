using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class EyeCampfire : WrongWarpBehaviour
    {
        const float DISAPPEAR_DISTANCE = 20f;

        public GameObject ShadowPuppets;

        bool disappeared = false;
        Campfire campfire;

        public override void WireUp()
        {
            campfire = gameObject.GetComponentInChildren<Campfire>();
            campfire.SetState(Campfire.State.LIT);
        }

        public void Update()
        {
            if (!disappeared)
            {
                var player = Locator.GetPlayerBody();
                if (Vector3.Distance(transform.position, player.transform.position) < DISAPPEAR_DISTANCE)
                {
                    HideCampfire(true);
                }
            }
        }

        public void HideCampfire(bool flicker)
        {
            disappeared = true;
            StartCoroutine(DoHideCampfire(flicker));
        }

        private IEnumerator DoHideCampfire(bool flicker)
        {
            if (flicker)
            {
                GlobalMessenger<float, float>.FireEvent("FlickerOffAndOn", 0.5f, 1f);
                yield return new WaitForSeconds(0.5f);
                campfire.SetState(Campfire.State.SMOLDERING);
            } else
            {
                campfire.SetState(Campfire.State.UNLIT);
                yield return new WaitForSeconds(0.5f);
                gameObject.SetActive(false);
            }
            ShadowPuppets.SetActive(false);
        }
    }
}
