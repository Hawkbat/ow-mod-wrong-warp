﻿using ModDataTools.Assets.Props;
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
    public class SensorEmitter : RadialSensor
    {
        public SignalPropAsset SignalAsset;
        public AudioSignal Signal;
        public OWAudioSource ActivationSound;

        public float TimeSinceLastActivation;

        public override void WireUp()
        {
            ActivationSound = GetComponent<OWAudioSource>();
        }

        public override float ComputeStrength() {
            var strength = Signal ? Signal.GetSignalStrength() * Signal.GetSignalStrength() : 0f;
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
                if (ActivationSound) ActivationSound.PlayOneShot(AudioType.NomaiOrbSlotActivated);
            }
            if (IsDeactivated(this))
            {
                TimeSinceLastActivation += Time.deltaTime;
            }

            float dist = Vector3.Distance(transform.position, Locator.GetPlayerBody().transform.position);
            if (Signal && dist > MaxDistance) Mod.DeviceSignals.DeallocateSignal(this);
            if (!Signal && dist < MinDistance) Mod.DeviceSignals.TryAllocateSignal(this);
        }
    }
}
