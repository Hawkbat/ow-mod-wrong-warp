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
        public Transform Scan;
        public float Duration;
        public float MaxRadius;
        public EasingFunc EaseIn;
        public EasingFunc EaseOut;

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
                float t = EasingUtils.Ease(ScanProgress, EaseIn, EaseOut);
                Scan.transform.localScale = Vector3.one * MaxRadius * t;
            }
        }
    }
}
