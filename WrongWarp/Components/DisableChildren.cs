using ModDataTools.Assets.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class DisableChildren : WrongWarpBehaviour
    {
        public List<Transform> Children = new();
        public List<string> ChildrenByName = new();

        void Awake()
        {
            Transform target = transform;
            var prop = gameObject.GetComponent<PropDataComponent>();
            if (prop != null)
            {
                var t = prop.GetSpawnedProp();
                if (t != null)
                {
                    target = t;
                }
            }
            foreach (var name in ChildrenByName)
            {
                Transform c = target.Find(name);
                if (c != null)
                {
                    Children.Add(c);
                }
            }
            foreach (var c in Children)
            {
                c.gameObject.SetActive(false);
            }
        }

        public override void WireUp()
        {
        }
    }
}
