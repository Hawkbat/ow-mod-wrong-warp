using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWUnityUtils
{
    [CreateAssetMenu]
    public class ShipLogRumorFact : ShipLogFactBase
    {
        [Tooltip("The source of this rumor, this draws a line in detective mode")]
        public ShipLogEntryBase Source;
        [Tooltip("Displays on the card in detective mode if no ExploreFacts have been revealed on the parent entry")]
        public string RumorName;
        [Tooltip("Priority over other RumorFacts to appear as the entry card's title")]
        public int RumorNamePriority;
    }
}
