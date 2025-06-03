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
        public float Delay;

        bool wiredUp;
        float timer;
        bool wasActive;

        public override void WireUp()
        {
            Objects = Objects.Select(o =>
            {
                var c = o.GetComponent<PropDataComponent>();
                var p = c != null ? c.GetSpawnedProp() : null;
                if (p != null) return p.gameObject;
                return o;
            }).ToList();
            wasActive = true;
            wiredUp = true;
        }

        protected void LateUpdate()
        {
            if (!wiredUp) return;
            var activated = Any ? Sensor.AreAnyActivated(Sensors) : Sensor.AreAllActivated(Sensors);
            var active = Invert ? !activated : activated;

            if (active == wasActive) return;

            timer += Time.deltaTime;

            if (timer >= Delay)
            {
                foreach (var obj in Objects)
                {
                    obj.SetActive(active);
                }
                wasActive = active;
                timer = 0f;
            }
        }
    }
}
