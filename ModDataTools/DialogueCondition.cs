using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWUnityUtils
{
    [CreateAssetMenu]
    public class DialogueCondition : ScriptableObject
    {
        public string Name { get => name; set => name = value; }
        public string OverrideID;
        public SolarSystem SolarSystem;
        public bool Persistent;

        public string GetID()
        {
            if (!string.IsNullOrEmpty(OverrideID)) return OverrideID;
            var id = "";
            if (SolarSystem) id += SolarSystem.ChildIDPrefix + "_";
            return id;
        }
    }
}
