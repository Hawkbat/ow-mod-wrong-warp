using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWUnityUtils
{
    public abstract class ShipLogEntryBase : ScriptableObject
    {
        public string Name { get => name; set => name = value; }
        [Tooltip("If set, uses this name instead of the filename")]
        public string OverrideName;
        [Tooltip("The ID of this entry")]
        public string IDSuffix;
        [Tooltip("If set, uses this name instead of generating an ID")]
        public string OverrideID;
        [Tooltip("The planet this entry is for")]
        public Planet Planet;
        [Tooltip("The position of the entry in rumor mode")]
        public Vector2 RumorModePosition;
        [Tooltip("Whether the entry has a physical location on the planet")]
        public bool HasPlanetLocation;
        [Tooltip("The path to a transform in the planet's prefab that marks the entry's location")]
        [ConditionalField(nameof(HasPlanetLocation))]
        public string PlanetLocationPath;
        [Tooltip("Whether to hide the \"More To Explore\" text on this entry")]
        public bool IgnoreMoreToExplore;
        [Tooltip("Ignore more to explore if a fact is known")]
        [ConditionalField(nameof(IgnoreMoreToExplore))]
        public ShipLogFactBase IgnoreMoreToExploreCondition;
        [Tooltip("The picture to show for the entry")]
        public Texture2D Photo;
        [Tooltip("An alt picture to show if a condition is met")]
        [ConditionalField(nameof(Photo))]
        public Texture2D AltPhoto;
        [Tooltip("If this fact is revealed, show the Alt picture")]
        [ConditionalField(nameof(AltPhoto))]
        public ShipLogFactBase AltPhotoCondition;

        public string GetName()
        {
            if (!string.IsNullOrEmpty(OverrideName)) return OverrideName;
            return name;
        }

        public string GetID()
        {
            if (!string.IsNullOrEmpty(OverrideID)) return OverrideID;
            var id = "";
            if (Planet && Planet.SolarSystem) id += Planet.SolarSystem.ChildIDPrefix + "_";
            if (Planet) id += Planet.ChildIDPrefix + "_";
            if (string.IsNullOrEmpty(IDSuffix)) id += name.Replace(' ', '_').ToUpper();
            else id += IDSuffix;
            return id;
        }

        public Vector2 EditorPosition
        {
            get => new Vector2(RumorModePosition.x, -RumorModePosition.y);
            set => RumorModePosition = new Vector2(value.x, -value.y);
        }

        public abstract ShipLogCuriosity GetCuriosity();

        public Vector3? GetPlanetLocation() => GetPlanetLocation(out string _);

        public Vector3? GetPlanetLocation(out string location)
        {
            location = "Invalid Planet";
            if (Planet)
            {
                location = "Invalid Planet Prefab";
                if (Planet.Prefab)
                {
                    location = "Invalid Planet Location Path";
                    Transform target = GetTransformAtPath(Planet.Prefab.transform, PlanetLocationPath);
                    if (target)
                    {
                        Vector3 p = Planet.Prefab.transform.InverseTransformPoint(target.transform.position);
                        location = target.name + " " + p.ToString();
                        return p;
                    }
                }
            }
            return null;
        }

        private Transform GetTransformAtPath(Transform transform, string path)
        {
            if (path == null) return null;
            var t = transform;
            var q = new Queue<string>(path.Split('/'));
            while (q.Count > 0 && t != null)
            {
                var s = q.Dequeue();
                if (s == "..") t = t.parent;
                else if (s != ".") t = t.Find(s);
            }
            return t;
        }
    }
}
