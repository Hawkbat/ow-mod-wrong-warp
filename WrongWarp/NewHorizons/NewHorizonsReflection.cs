using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Utils;

namespace WrongWarp.NewHorizons
{
    public static class NewHorizonsReflection
    {
        static Type mainType;
        static object mainInstance;

        public static void ChangeCurrentStarSystem(string newStarSystem, bool warp = false, bool vessel = false)
        {
            var method = GetMainType().GetMethod("ChangeCurrentStarSystem", BindingFlags.Public | BindingFlags.Instance);
            method.Invoke(GetMainInstance(), [newStarSystem, warp, vessel]);
        }

        public static bool DidWarpFromVessel
        {
            set
            {
                var prop = GetMainType().GetProperty("DidWarpFromVessel", BindingFlags.Public | BindingFlags.Instance);
                prop.SetMethod.Invoke(GetMainInstance(), [value]);
            }
        }

        static Type GetMainType()
        {
            if (mainType != null) return mainType;
            mainType = Type.GetType("NewHorizons.Main, NewHorizons");
            return mainType;
        }

        static object GetMainInstance()
        {
            if (mainInstance != null) return mainInstance;
            var instanceProp = GetMainType().GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            mainInstance = instanceProp.GetValue(null);
            return mainInstance;
        }
    }
}
