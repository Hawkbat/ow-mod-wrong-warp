using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWUnityUtils
{
    public abstract class ShipLogFactBase : ScriptableObject
    {
        public string Name { get => name; set => name = value; }
        [Tooltip("If set, uses this name instead of the filename")]
        public string OverrideName;
        [Tooltip("The ID of this explore fact")]
        public string IDSuffix;
        [Tooltip("If set, uses this name instead of the generated ID")]
        public string OverrideID;
        [Tooltip("The entry this fact belongs to")]
        public ShipLogEntryBase Entry;
        [Tooltip("Whether to hide the \"More to explore\" for this fact")]
        public bool IgnoreMoreToExplore;
        [Tooltip("The text content for this fact")]
        [TextArea]
        public string Text;
        [Tooltip("The condition that needs to be fulfilled to have the alt text be displayed")]
        public ShipLogFactBase AltTextCondition;
        [Tooltip("The text to display if the condition is met")]
        [TextArea]
        public string AltText;
        [Tooltip("Reveal this fact when the game starts")]
        public bool InitiallyRevealed;

        public string GetID()
        {
            if (!string.IsNullOrEmpty(OverrideID)) return OverrideID;
            var id = "";
            if (Entry) id += Entry.GetID() + "_";
            if (string.IsNullOrEmpty(IDSuffix)) id += name.Replace(' ', '_').ToUpper();
            else id += IDSuffix;
            return id;
        }
    }
}
