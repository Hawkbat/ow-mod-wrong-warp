using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWUnityUtils
{
    [CreateAssetMenu]
    public class ModDefinition : ScriptableObject
    {
        public string Name { get => name; set => name = value; }
        public string UniqueName;
        public string Author;
        public string FileName;
        public string Version;
        public string OWMLVersion;
        public string Patcher;
        public string[] Dependencies;
        public bool PriorityLoad;
        public string MinGameVersion;
        public string MaxGameVersion;
        public bool RequireLatestVersion;
        public Vendors IncompatibleVendors;
        public string[] PathsToPreserve;
        public string[] Conflicts;
        public string Warning;

        public enum Vendors
        {
            None = 0,
            Steam = 1 << 0,
            Epic = 1 << 1,
            Gamepass = 1 << 2,
        }
    }
}
