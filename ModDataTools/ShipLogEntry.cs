using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWUnityUtils
{
    [CreateAssetMenu]
    public class ShipLogEntry : ShipLogEntryBase
    {
        [Tooltip("The entry this entry is a child of")]
        public ShipLogEntryBase Parent;
        [Tooltip("The curiosity this entry belongs to")]
        public ShipLogCuriosity Curiosity;

        public override ShipLogCuriosity GetCuriosity() => Curiosity;
    }
}
