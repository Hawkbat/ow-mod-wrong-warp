using ModDataTools.Assets.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class SensorGameObjectToggle : WrongWarpBehaviour
    {
        public List<GameObject> Objects = new();
        public List<Sensor> Sensors = new();
        public bool Any;
        public bool Invert;

        bool wiredUp;

        public override void WireUp()
        {
            Objects = Objects.Select(o =>
            {
                var c = o.GetComponent<PropDataComponent>();
                var p = c.GetSpawnedProp();
                if (p != null) return p.gameObject;
                return o;
            }).ToList();
            wiredUp = true;
        }

        void LateUpdate()
        {
            if (!wiredUp) return;
            var activated = Any ? Sensor.AreAnyActivated(Sensors) : Sensor.AreAllActivated(Sensors);
            var active = Invert ? !activated : activated;
            foreach (var obj in Objects)
            {
                obj.SetActive(active);
            }
        }
    }
}
