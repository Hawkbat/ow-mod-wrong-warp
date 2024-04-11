using ModDataTools.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Utils
{
    public static class DialogueUtils
    {
        public static bool AllConditionsMet(IEnumerable<ConditionAsset> conditions)
            => !conditions.Any() || conditions.All(c => ConditionMet(c));

        public static bool AnyConditionsMet(IEnumerable<ConditionAsset> conditions)
            => conditions.Any() && conditions.Any(c => ConditionMet(c));

        public static bool ConditionMet(ConditionAsset condition)
        {
            if (condition == null) return true;
            if (condition.Persistent) return PlayerData.GetPersistentCondition(condition.FullID);
            return DialogueConditionManager.SharedInstance.GetConditionState(condition.FullID);
        }

        public static void SetConditions(IEnumerable<ConditionAsset> conditions, bool state)
        {
            foreach (var condition in conditions) SetCondition(condition, state);
        }

        public static void SetCondition(ConditionAsset condition, bool state)
        {
            if (condition.Persistent)
                PlayerData.SetPersistentCondition(condition.FullID, state);
            else
                DialogueConditionManager.SharedInstance.SetConditionState(condition.FullID, state);
        }

        public static bool AllFactsRevealed(IEnumerable<FactAsset> facts)
            => !facts.Any() || facts.All(f => FactRevealed(f));

        public static bool AnyFactsRevealed(IEnumerable<FactAsset> facts)
            => facts.Any() && facts.Any(f => FactRevealed(f));

        public static bool FactRevealed(FactAsset fact)
        {
            if (fact == null) return true;
            return Locator.GetShipLogManager().IsFactRevealed(fact.FullID);
        }

        public static void RevealFacts(IEnumerable<FactAsset> facts)
        {
            foreach (FactAsset fact in facts) RevealFact(fact);
        }

        public static void RevealFact(FactAsset fact)
        {
            Locator.GetShipLogManager().RevealFact(fact.FullID);
        }
    }
}
