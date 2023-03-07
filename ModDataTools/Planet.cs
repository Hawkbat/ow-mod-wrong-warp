using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWUnityUtils
{
    [CreateAssetMenu]
    public class Planet : ScriptableObject
    {
        public string Name { get => name; set => name = value; }
        [Tooltip("The solar system this planet belongs in")]
        public SolarSystem SolarSystem;
        [Tooltip("A prefix appended to all entry and fact IDs belonging to this planet")]
        public string ChildIDPrefix;
        [Tooltip("The main prefab representing the terrain for this planet, for positioning ship log locations")]
        public GameObject Prefab;
        [Tooltip("The New Horizons planet config .json file")]
        public TextAsset ConfigFile;
    }
}
