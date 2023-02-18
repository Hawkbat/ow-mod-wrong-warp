using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp;
using WrongWarp.Configs;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public abstract class WrongWarpBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public WrongWarpObject ModObject;

        public WrongWarpMod Mod => ModObject?.Mod;

        public abstract void WireUp();

        public virtual IEnumerator Start()
        {
            if (!ModObject) ModObject = gameObject.GetAddComponent<WrongWarpObject>();
            yield return null;
            WireUp();
        }

        public Transform GetTransformAtPath(string path)
            => UnityUtils.GetTransformAtPath(transform, path);

        public TComponent GetComponentAtPath<TComponent>(string path) where TComponent : Component
            => UnityUtils.GetComponentAtPath<TComponent>(transform, path);

        public List<TComponent> GetComponentsAtPaths<TComponent>(List<string> paths) where TComponent : Component
            => UnityUtils.GetComponentsAtPaths<TComponent>(transform, paths);
    }
}
