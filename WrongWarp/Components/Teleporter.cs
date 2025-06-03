using ModDataTools.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class Teleporter : Sensor
    {
        static List<Teleporter> all = new();

        public GameObject EffectPrefab;
        public float Duration;
        public Teleporter Linked;
        public FrequencyAsset Frequency;
        public List<ConditionAsset> ConditionsToSet = new List<ConditionAsset>();

        private bool isTeleporting;
        private List<GameObject> occupants = new List<GameObject>();
        private OWTriggerVolume triggerVolume;
        private bool isReEnable;

        protected void OnEnable()
        {
            all.Add(this);
            triggerVolume = GetComponentInChildren<OWTriggerVolume>(true);
            triggerVolume.OnEntry += TriggerVolume_OnEntry;
            triggerVolume.OnExit += TriggerVolume_OnExit;
            if (isReEnable)
            {
                AttemptLink();
                isReEnable = false;
            }
        }

        protected void OnDisable()
        {
            all.Remove(this);
            triggerVolume.OnEntry -= TriggerVolume_OnEntry;
            triggerVolume.OnExit -= TriggerVolume_OnExit;
            AttemptUnlink();
            isReEnable = true;
        }

        public override void WireUp()
        {
            AttemptLink();
        }

        public override float ComputeStrength()
            => Linked && Linked.isActiveAndEnabled ? 1f : 0f;

        void AttemptLink()
        {
            if (!Linked)
            {
                var other = all.Find(t => t && t.isActiveAndEnabled && t != this && t.Frequency && Frequency && t.Frequency.FullID == Frequency.FullID);
                if (other)
                {
                    Linked = other;
                    other.Linked = this;
                }
            }
        }

        void AttemptUnlink()
        {
            if (Linked)
            {
                Linked.Linked = null;
                if (Linked.isActiveAndEnabled) Linked.AttemptLink();
                Linked = null;
            }
        }

        private void TriggerVolume_OnEntry(GameObject hitObj)
        {
            var rb = hitObj.GetAttachedOWRigidbody();
            if (rb && rb.CompareTag("Player"))
            {
                if (!occupants.Contains(rb.gameObject))
                {
                    occupants.Add(rb.gameObject);
                    if (!isTeleporting && Linked)
                    {
                        Teleport(Linked.transform, rb.transform);
                    }
                }
            }
        }

        private void TriggerVolume_OnExit(GameObject hitObj)
        {
            var rb = hitObj.GetAttachedOWRigidbody();
            if (rb && rb.CompareTag("Player"))
            {
                if (occupants.Contains(rb.gameObject))
                    occupants.Remove(rb.gameObject);
            }
        }

        public void Teleport(Transform target, Transform subject)
        {
            StartCoroutine(DoTeleport(target, subject));
        }

        private IEnumerator DoTeleport(Transform target, Transform subject)
        {
            isTeleporting = true;
            if (Linked && target == Linked.transform) Linked.isTeleporting = true;

            var rb = subject.GetAttachedOWRigidbody();

            var sourceEffect = Instantiate(EffectPrefab, transform.position, transform.rotation, transform).GetComponentInChildren<TeleporterEffect>();
            sourceEffect.Duration = Duration;
            sourceEffect.gameObject.SetActive(true);
            var targetEffect = Instantiate(EffectPrefab, target.position, target.rotation, target).GetComponentInChildren<TeleporterEffect>();
            targetEffect.Duration = Duration;
            targetEffect.gameObject.SetActive(true);

            if (rb == Locator.GetPlayerBody())
            {
                //Mod.QuantumExhibit.ApplyTemporaryLock();
                Mod.Respawner.MakeCameraParticles(Duration);

                DialogueUtils.SetConditions(ConditionsToSet, true);
            }

            yield return new WaitForSeconds(Duration);

            if (rb == Locator.GetProbe().GetAttachedOWRigidbody())
            {
                if (Locator.GetProbe().IsAnchored())
                    Locator.GetProbe().Unanchor();
            }

            var pos = transform.InverseTransformPoint(rb ? rb.GetPosition() : subject.position);
            var rot = transform.InverseTransformRotation(rb ? rb.GetRotation() : subject.rotation);

            pos = target.TransformPoint(pos);
            rot = target.rotation * rot;

            if (rb)
            {
                rb.WarpToPositionRotation(pos, rot);
                rb.SetVelocity(target.GetAttachedOWRigidbody().GetPointVelocity(pos));
            }
            else
            {
                subject.position = pos;
                subject.rotation = rot;
            }

            yield return new WaitForSeconds(Duration);

            if (rb == Locator.GetPlayerBody())
            {
                //Mod.QuantumExhibit.ReleaseTemporaryLock();
            }

            isTeleporting = false;
            if (Linked && target == Linked.transform) Linked.isTeleporting = false;
        }
    }
}
