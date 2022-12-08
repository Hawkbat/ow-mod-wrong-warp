using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class WrongWarpObject : MonoBehaviour
    {
        public WrongWarpMod Mod;
        public List<string> ObjectTypes = new List<string>();

        public void Awake()
        {
            if (!Mod) Mod = FindObjectOfType<WrongWarpMod>();
            var objectTypesToAdd = new List<string>(ObjectTypes);
            foreach (var objectType in objectTypesToAdd) Mod.ApplyModObjectType(gameObject, objectType);
        }
    }
}
