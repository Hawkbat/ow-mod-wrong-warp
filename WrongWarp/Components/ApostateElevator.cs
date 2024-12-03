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
        float Offset;
        Animator Anim;

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
                    foo.transform.localPosition = Vector3.zero;
                    foo.transform.localRotation = Quaternion.identity;
                    foo.transform.localScale = Vector3.one;
                    Anim = foo.GetComponent<Animator>();
                    Offset = (float)i / SegQuantity;
                    if (Anim) { Anim.SetFloat("Offset", Offset); } else LogUtils.Log("Animator not Found");
                }
            }
            else
            {
                LogUtils.Log("SegQuantity must be greater than 0");
            }
        }
    }
}
