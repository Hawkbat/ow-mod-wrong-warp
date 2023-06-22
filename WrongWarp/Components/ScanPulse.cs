using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class ScanPulse : WrongWarpBehaviour, IConfigurable<ScanPulseConfig>
    {
        public Transform Target;
        public Transform Scan;
        public float Duration;
        public float MaxRadius;
        public EasingType EaseInType;
        public EasingFunc EaseIn;
        public EasingType EaseOutType;
        public EasingFunc EaseOut;
        public bool Reverse;

        public float ScanProgress;

        public bool IsScanning => ScanProgress > 0f;

        public void ApplyConfig(ScanPulseConfig config)
        {
            if (config.duration.HasValue) Duration = config.duration.Value;
            if (config.maxRadius.HasValue) MaxRadius = config.maxRadius.Value;
            if (config.easeIn != null) EaseIn = EasingUtils.Lookup(config.easeIn);
            if (config.easeOut != null) EaseOut = EasingUtils.Lookup(config.easeOut);
        }

        public override void WireUp()
        {
            if (EaseIn == null) EaseIn = EasingUtils.Lookup(EaseInType);
            if (EaseOut == null) EaseOut = EasingUtils.Lookup(EaseOutType);
            Scan = GetTransformAtPath("Scan");
        }

        public void StartScan()
        {
            ScanProgress = Mathf.Epsilon;
        }

        public void StopScan()
        {
            ScanProgress = 0f;
        }

        public void Update()
        {
            ScanProgress += Time.deltaTime / Duration;
            if (ScanProgress > 1f) StopScan();
            if (Scan)
            {
                float t = EasingUtils.Ease(Reverse ? 1f - ScanProgress : ScanProgress, EaseIn, EaseOut);
                Scan.transform.localScale = Vector3.one * MaxRadius * t;
                if (Target)
                {
                    float t2 = EasingUtils.Ease(Reverse ? 1f - ScanProgress : ScanProgress, EasingUtils.Linear, EasingUtils.Octic);
                    Scan.transform.position = Vector3.Lerp(transform.position, Target.position, Reverse ? 1f - ScanProgress : ScanProgress);
                }
            }
        }
    }
}
