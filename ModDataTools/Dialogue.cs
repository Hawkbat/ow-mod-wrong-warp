using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWUnityUtils
{
    [CreateAssetMenu]
    public class Dialogue : ScriptableObject
    {
        public string Name { get => name; set => name = value; }
        public SolarSystem SolarSystem;
        public DialogueType Type;
        [ConditionalField(nameof(Type), DialogueType.Character)]
        public string CharacterName;
        public List<Node> Nodes;
        public TextAsset RawXml;

        public enum DialogueType
        {
            Unknown = 0,
            Character = 1,
            Sign = 2,
            Recording = 3,
        }

        public string GetID()
        {
            var id = "";
            if (SolarSystem) id += SolarSystem.ChildIDPrefix + "_";
            id += name.Replace(' ', '_').ToUpper();
            return id;
        }

        [Serializable]
        public class Node
        {
            public string Name;
            public DialogueCondition EntryCondition;
            public bool Randomize;
            public List<string> Pages;
            public List<Option> Options;
            public List<ShipLogFactBase> RevealFacts;
            public List<DialogueCondition> SetConditions;
            public List<ShipLogFactBase> RequiredTargetFacts;
            public Node Target;

            public string GetID(Dialogue parent)
            {
                var id = "";
                if (parent) id += parent.GetID() + "_";
                id += Name.Replace(' ', '_').ToUpper();
                return id;
            }
        }

        [Serializable]
        public class Option
        {
            public string Text;
            public List<DialogueCondition> RequiredConditions;
            public List<DialogueCondition> CancelledConditions;
            public List<ShipLogFactBase> RequiredFacts;
            public List<DialogueCondition> SetConditions;
            public List<DialogueCondition> ConditionsToCancel;
            public Node Target;
        }
    }
}
