using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class ApostateElevator : WrongWarpBehaviour
    {
        public GameObject Segment;
        public int SegQuantity;

        public override void WireUp()
        {
            Transform pt = gameObject.transform;
            LogUtils.Log("Setting Up Elevator");
            if (SegQuantity > 0)
            {
                for (int i = 0; i < SegQuantity; i++)
                {
                    LogUtils.Log("Setting Up Elevator Platform " + (i + 1));
                    GameObject foo = Instantiate(Segment, pt);
                    foo.transform.position = Vector3.zero;
                    foo.transform.rotation = Quaternion.identity;
                    foo.transform.localScale = Vector3.one;
                    Animator anim = foo.GetComponent<Animator>();
                    if (anim) { anim.SetFloat("Cycle Offset", i / SegQuantity); } else LogUtils.Log("Animator not Found");
                }
            }
            else
            {
                LogUtils.Log("SegQuantity must be greater than 0");
            }
        }
    }
}
