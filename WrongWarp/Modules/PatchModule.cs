using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OWML.Common;
using UnityEngine;
using WrongWarp.Utils;
using HarmonyLib;

namespace WrongWarp.Modules
{
    [HarmonyPatch]
    public class PatchModule : WrongWarpModule
    {
        public PatchModule(WrongWarpMod mod) : base(mod)
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
