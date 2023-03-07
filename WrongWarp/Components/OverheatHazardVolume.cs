using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class OverheatHazardVolume : HazardVolume
    {
        public float MinRadius;
        public float MaxRadius;
        public AnimationCurve RadiusOverTimeCurve;
        public float MinTemperature;
        public float MaxTemperature;
        public AnimationCurve TemperatureOverTimeCurve;
        public AnimationCurve TemperatureOverDistanceCurve;
        public float MinDamagePerSecond;
        public float MaxDamagePerSecond;
        public AnimationCurve DamageOverTemperatureCurve;
        public float SuitlessDamageReduction;
        public float SuitedDamageReduction;
        public float InShipDamageReduction;

        public override HazardType GetHazardType() => HazardType.HEAT;
        public override InstantDamageType GetFirstContactDamageType() => InstantDamageType.Impact;

        SphereShape shape;

        public override void Awake()
        {
            base.Awake();
            shape = GetComponent<SphereShape>();
        }

        void Update()
        {
            shape.radius = GetRadius();
        }

        public override float GetDamagePerSecond(HazardDetector detector)
        {
            if (detector._isPlayerDetector)
            {
                return Mathf.Max(0f, GetEffectiveDamageToPlayer());
            }
            return Mathf.Max(0f, GetRawDamageAt(detector.transform.position));
        }

        public float GetProgress() =>
            TimeLoop.GetFractionElapsed();
        public float GetRadius() =>
            Mathf.Lerp(MinRadius, MaxRadius, GetProgress());
        public float GetDistance(Vector3 point) =>
            Vector3.Distance(transform.position, point);
        public float GetCurrentMaxTemperature()
            => Mathf.Lerp(MinTemperature, MaxTemperature, TemperatureOverTimeCurve.Evaluate(GetProgress()));
        public float GetTemperatureAt(Vector3 point) =>
            Mathf.Lerp(MinTemperature, GetCurrentMaxTemperature(), TemperatureOverDistanceCurve.Evaluate(Mathf.InverseLerp(GetRadius(), MinRadius, GetDistance(point))));
        public float GetCurrentMaxRawDamage()
            => Mathf.Lerp(MinDamagePerSecond, MaxDamagePerSecond, DamageOverTemperatureCurve.Evaluate(Mathf.InverseLerp(MinTemperature, MaxTemperature, GetCurrentMaxTemperature())));
        public float GetRawDamageAt(Vector3 point) =>
            Mathf.Lerp(MinDamagePerSecond, MaxDamagePerSecond, DamageOverTemperatureCurve.Evaluate(Mathf.InverseLerp(MinTemperature, MaxTemperature, GetTemperatureAt(point))));
        
        public float GetEffectiveDamageToPlayer()
        {
            var damage = GetRawDamageAt(Locator.GetPlayerTransform().position);
            damage = Mathf.Clamp(damage, 0f, MaxDamagePerSecond);
            if (Locator.GetPlayerSuit().IsWearingSuit())
            {
                damage -= SuitedDamageReduction;
            } else if (Locator.GetPlayerSectorDetector().IsWithinSector(Sector.Name.Ship))
            {
                damage -= InShipDamageReduction;
            } else
            {
                damage -= SuitlessDamageReduction;
            }
            return damage;
        }
    }
}
