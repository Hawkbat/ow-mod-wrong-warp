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
    public class MotionShaft : WrongWarpBehaviour, IConfigurable<MotionShaftConfig>
    {
        public List<Sensor> Sensors = new List<Sensor>();
        public List<string> SensorPaths = new List<string>();
        public TractorBeamFluid TractorBeam;
        public DirectionalForceVolume ForceVolume;
        public Transform StartPoint;
        public Transform EndPoint;
        public GameObject IndicatorPrefab;
        public int IndicatorCount;
        public float IndicatorSpeed;
        public float Force;
        public bool Reversed;

        public List<Indicator> Indicators = new List<Indicator>();

        public void ApplyConfig(MotionShaftConfig config)
        {
            if (config.sensorPaths != null) SensorPaths = config.sensorPaths;
            if (config.force.HasValue) Force = config.force.Value;
            if (config.indicatorCount.HasValue) IndicatorCount = config.indicatorCount.Value;
            if (config.indicatorSpeed.HasValue) IndicatorSpeed = config.indicatorSpeed.Value;
            if (config.startReversed.HasValue) Reversed = config.startReversed.Value;
        }

        public override void WireUp()
        {
            Sensors = GetComponentsAtPaths<Sensor>(SensorPaths);
            TractorBeam = GetComponent<TractorBeamFluid>();
            ForceVolume = GetComponent<DirectionalForceVolume>();
            IndicatorPrefab = GetTransformAtPath("Indicator").gameObject;
            IndicatorPrefab.SetActive(false);
            StartPoint = GetTransformAtPath("Start");
            EndPoint = GetTransformAtPath("End");

            for (int i = 0; i < IndicatorCount; i++)
            {
                Transform indicator = Instantiate(IndicatorPrefab).GetComponent<Transform>();
                indicator.gameObject.SetActive(true);
                indicator.SetParent(transform);
                indicator.localRotation = IndicatorPrefab.transform.localRotation;
                indicator.localScale = IndicatorPrefab.transform.localScale;
                Indicators.Add(new Indicator()
                {
                    transform = indicator,
                    t = Mathf.InverseLerp(0f, IndicatorCount, i),
                });
            }
        }

        public void Update()
        {
            if (!StartPoint || !EndPoint) return;
            if (Sensor.AreAnyActivated(Sensors))
            {
                Reversed = !Reversed;
            }
            
            Vector3 dir = (EndPoint.position - StartPoint.position).normalized;
            if (TractorBeam)
            {
                if (Reversed != TractorBeam.IsFluidReversed())
                    TractorBeam.SetFluidReversed(Reversed);
                TractorBeam._verticalSpeed = Force;
                TractorBeam._reverseSpeed = -Force;
            }
            if (ForceVolume)
            {
                float dot = Reversed ? -1f : 1f;
                ForceVolume.SetFieldMagnitude(Force);
                ForceVolume.SetLocalForceDirection(transform.InverseTransformDirection((dir * dot).normalized));
            }
            foreach (Indicator indicator in Indicators)
            {
                indicator.t = (indicator.t + Time.deltaTime * IndicatorSpeed * (Reversed ? -1f : 1f) + 10000f) % 1f;
                indicator.transform.up = dir;
                indicator.transform.position = Vector3.Lerp(StartPoint.position, EndPoint.position, indicator.t);
            }
        }

        [System.Serializable]
        public class Indicator
        {
            public Transform transform;
            public float t;
        }
    }
}
