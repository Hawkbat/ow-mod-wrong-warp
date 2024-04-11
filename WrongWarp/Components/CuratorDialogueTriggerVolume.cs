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
    [RequireComponent(typeof(OWTriggerVolume))]
    public class CuratorDialogueTriggerVolume : WrongWarpBehaviour
    {
        public DialogueAsset Dialogue;
        public List<ConditionAsset> RequiredConditions = new List<ConditionAsset>();
        public List<ConditionAsset> CancelledConditions = new List<ConditionAsset>();
        public List<FactAsset> RequiredFacts = new List<FactAsset>();
        public List<FactAsset> CancelledFacts = new List<FactAsset>();
        public bool Repeatable;
        public float Cooldown;

        OWTriggerVolume trigger;
        int triggerCount;
        float triggerTime;

        public override void WireUp()
        {
            trigger = gameObject.GetRequiredComponent<OWTriggerVolume>();
            trigger.OnEntry += OnEntry;
        }

        protected void OnDestroy()
        {
            trigger.OnEntry -= OnEntry;
        }

        private void OnEntry(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                var conditionsMet = true;
                if (triggerCount == 0 || Repeatable)
                {
                    conditionsMet = conditionsMet && (triggerCount == 0 || Time.time > triggerTime + Cooldown);
                    conditionsMet = conditionsMet && DialogueUtils.AllConditionsMet(RequiredConditions);
                    conditionsMet = conditionsMet && !DialogueUtils.AnyConditionsMet(CancelledConditions);
                    conditionsMet = conditionsMet && DialogueUtils.AllFactsRevealed(RequiredFacts);
                    conditionsMet = conditionsMet && !DialogueUtils.AnyFactsRevealed(CancelledFacts);
                    if (conditionsMet)
                    {
                        Mod.Curator.TriggerDialogue(Dialogue);
                        triggerTime = Time.time;
                        triggerCount++;
                    }
                }
            }
        }
    }
}
