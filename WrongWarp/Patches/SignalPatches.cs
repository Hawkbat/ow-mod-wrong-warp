using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SignalTowerType = WrongWarp.Modules.SignalTowerModule.SignalTowerType;

namespace WrongWarp.Patches
{
    [HarmonyPatch]
    public static class SignalPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlayerData), nameof(PlayerData.KnowsSignal))]
        public static bool PlayerData_KnowsSignal(SignalName __0, ref bool __result)
        {
            if (WrongWarpMod.Instance.DeviceSignals.IsManagedSignal(__0))
            {
                __result = WrongWarpMod.Instance.DeviceSignals.IsSignalKnown(__0);
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerData), nameof(PlayerData.LearnSignal))]
        public static bool PlayerData_LearnSignal(SignalName __0)
        {
            if (WrongWarpMod.Instance.DeviceSignals.IsManagedSignal(__0))
            {
                WrongWarpMod.Instance.DeviceSignals.LearnSignal(__0);
                return false;
            }
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AudioSignal), nameof(AudioSignal.UpdateSignalStrength))]
        public static void AudioSignal_UpdateSignalStrength(AudioSignal __instance, Signalscope scope, float distToClosestScopeObstruction)
        {
            var towers = WrongWarpMod.Instance.SignalTowers;
            if (__instance != towers.GetDirelictSignal()) return;
            
            var towerActiveA = towers.IsSignalTowerActive(SignalTowerType.Archivist);
            var towerActiveB = towers.IsSignalTowerActive(SignalTowerType.Guide);
            var towerActiveC = towers.IsSignalTowerActive(SignalTowerType.Curator);
            var towerA = towers.GetSignalTowerBody(SignalTowerType.Archivist);
            var towerB = towers.GetSignalTowerBody(SignalTowerType.Guide);
            var towerC = towers.GetSignalTowerBody(SignalTowerType.Curator);
            var towerDistA = towers.GetSignalTowerDistance(SignalTowerType.Archivist);
            var towerDistB = towers.GetSignalTowerDistance(SignalTowerType.Guide);
            var towerDistC = towers.GetSignalTowerDistance(SignalTowerType.Curator);

            var activeTowers = new List<OWRigidbody>();
            if (towerActiveA) activeTowers.Add(towerA);
            if (towerActiveB) activeTowers.Add(towerB);
            if (towerActiveC) activeTowers.Add(towerC);

            var simulatedPosition = __instance.transform.position;

            if (activeTowers.Count == 0)
            {
                // No tracking towers active; signal is "hidden"
                __instance._signalStrength = 0f;
                __instance._degreesFromScope = 180f;
            }
            else if (activeTowers.Count == 1)
            {
                // One tower active; signal area is the surface of a sphere around the tower

            }
            else if (activeTowers.Count == 2)
            {
                // Two towers active; signal area is the intersecting points of two spheres, a circle
                
            }
            else if (activeTowers.Count == 3)
            {
                // Three towers active; signal area is the intersecting points of three spheres, two points
                var (valid, p0, p1) = Trilaterate(towerA.GetPosition(), towerDistA, towerB.GetPosition(), towerDistB, towerC.GetPosition(), towerDistC);
                if (valid)
                {
                    simulatedPosition = UnityEngine.Random.value > 0.5f ? p1 : p0;
                }
            }

            var actualDiff = __instance.transform.position - scope.transform.position;
            
        }

        static (bool, Vector3, Vector3) Trilaterate(Vector3 p1, float r1, Vector3 p2, float r2, Vector3 p3, float r3)
        {
            Vector3 ex = (p2 - p1).normalized;
            float i = Vector3.Dot(ex, p3 - p1);
            Vector3 a = (p3 - p1) - (ex * i);
            Vector3 ey = a.normalized;
            Vector3 ez = Vector3.Cross(ex, ey);
            float d = (p2 - p1).magnitude;
            float j = Vector3.Dot(ey, p3 - p1);

            float x = ((r1 * r1) - (r2 * r2) + (d * d)) / (2 * d);
            float y = ((r1 * r1) - (r3 * r3) + (i * i) + (j * j)) / (2 * j) - (i / j) * x;

            float b = (r1 * r1) - (x * x) - (y * y);

            if (Mathf.Abs(b) < 0.0000000001)
            {
                b = 0;
            }

            float z = Mathf.Sqrt(b);

            if (float.IsNaN(z))
            {
                return (false, Vector3.zero, Vector3.zero);
            }

            Vector3 aa = p1 + ((ex * x) + (ey * y));
            Vector3 p4a = (aa + (ez * z));
            Vector3 p4b = (aa - (ez * z));

            return (true, p4a, p4b);
        }
    }
}
