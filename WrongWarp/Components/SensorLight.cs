using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;
using WrongWarp.Modules;

namespace WrongWarp.Components
{
    public class SensorLight : WrongWarpBehaviour, IConfigurable<SignalLightConfig>
    {
        public List<Sensor> Sensors = new List<Sensor>();
        public List<string> SensorPaths = new List<string>();
        public MeshRenderer LightOn;
        public MeshRenderer LightOff;

        Material matOn;
        Material matOff;
        Material mat;

        public bool IsOn => Sensor.AreAllActivated(Sensors);

        public void ApplyConfig(SignalLightConfig config)
        {
            if (config.sensorPaths != null) SensorPaths = config.sensorPaths;
        }

        public override void WireUp()
        {
            if (!Sensors.Any() && SensorPaths.Any()) Sensors = GetComponentsAtPaths<Sensor>(SensorPaths);
            if (!LightOn) LightOn = GetTransformAtPath("LightOn").GetComponent<MeshRenderer>();
            if (!LightOff) LightOff = GetTransformAtPath("LightOff").GetComponent<MeshRenderer>();
            if (LightOn) matOn = LightOn.sharedMaterial;
            if (LightOff) matOff = LightOff.sharedMaterial;
            if (matOff) mat = new Material(matOff);
        }

        public void Update()
        {
            if (LightOn) LightOn.gameObject.SetActive(IsOn);
            if (LightOff)
            {
                if (mat && matOn && matOff)
                {
                    var s = Sensor.GetMaxStrength(Sensors);
                    if (s > 0f)
                    {
                        var colorOn = matOn.GetColor("_EmissionColor");
                        var colorOff = matOff.GetColor("_EmissionColor");
                        mat.SetColor("_EmissionColor", Color.Lerp(colorOff, colorOn, s * 0.5f));
                        LightOff.sharedMaterial = mat;

                    }
                    else
                    {
                        LightOff.sharedMaterial = matOff;
                    }
                }
                LightOff.gameObject.SetActive(!IsOn);
            }
        }
    }
}
