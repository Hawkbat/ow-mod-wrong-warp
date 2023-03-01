using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class MissingExhibit : WrongWarpBehaviour
    {
        public GameObject Debris;
        public GameObject Exhibit;

        public override void WireUp()
        {

        }

        void Update()
        {
            bool restored = Mod.SaveData.ExhibitRestored;
            if (Debris.activeSelf != !restored) Debris.SetActive(!restored);
            if (Exhibit.activeSelf != restored) Exhibit.SetActive(restored);
        }
    }
}
