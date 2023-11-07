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
    public class SensorFakeSignal : RadialSensor
    {
        public OWAudioSource ActivationSound;

        public float TimeSinceLastActivation;

        public override void WireUp()
        {
            if (!ActivationSound) ActivationSound = GetComponent<OWAudioSource>();
        }

        public override float ComputeStrength() {
            var strength = 0f;
            var scope = Locator.GetToolModeSwapper().GetSignalScope();
            if (scope.IsEquipped())
            {
                var scopeToSignal = transform.position - scope.transform.position;
                var dist = scopeToSignal.magnitude;
                var angle = Vector3.Angle(scope.GetScopeDirection(), scopeToSignal);
                strength = Mathf.Clamp01(Mathf.InverseLerp(45f, 5f, angle));
                if (dist < MinDistance) strength = 1f;
                if (dist > MaxDistance) strength = 0f;
            }
            if (strength >= 1f && TimeSinceLastActivation < 0.25f)
            {
                TimeSinceLastActivation = 0f;
                return 0.99f;
            }
            return strength;
        }

        public override void Update()
        {
            base.Update();

            if (WasActivatedThisFrame(this))
            {
                if (ActivationSound) ActivationSound.Play();
            }
            if (IsDeactivated(this))
            {
                TimeSinceLastActivation += Time.deltaTime;
            }
        }
    }
}
