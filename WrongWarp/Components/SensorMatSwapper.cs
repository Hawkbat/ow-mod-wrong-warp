using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class SensorMatSwapper : WrongWarpBehaviour
    {
        public bool All;
        public List<Sensor> Sensors = new();
        public Material ActiveMaterial;
        public Material InactiveMaterial;
        public int MaterialSlot;

        bool wasActive;

        public override void WireUp()
        {
            Swap(false);
        }

        public void Update()
        {
            var sensorsActive = All ? Sensor.AreAllActivated(Sensors) : Sensor.AreAnyActivated(Sensors);
            if (sensorsActive != wasActive)
            {
                Swap(sensorsActive);
                wasActive = sensorsActive;
            }
        }

        void Swap(bool sensorsActive)
        {
            var mat = sensorsActive ? ActiveMaterial : InactiveMaterial;
            foreach (var r in GetComponents<Renderer>())
            {
                var arr = r.sharedMaterials;
                arr[MaterialSlot] = mat;
                r.sharedMaterials = arr;
            }
        }
    }
}
