using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class VanillaStationaryProbeLauncher : WrongWarpBehaviour
    {
        public string PropPath = "TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/TutorialCamera_Base/";
        public float InitialDegreesY;
        public bool LockInputY;
        public bool PhotosOnly;
        public bool ReturnToStartPos;

        public Vector3 interactPointOffset;

        private GameObject prop;
        public override void WireUp()
        {
            if (!string.IsNullOrEmpty(PropPath))
            {
                prop = Mod.NewHorizonsApi.SpawnObject(Mod, transform.root.gameObject, GetComponentInParent<Sector>(), PropPath, Vector3.zero, Vector3.zero, 1f, false);
                prop.transform.SetParent(transform, false);
                prop.transform.localPosition = Vector3.zero;
                prop.transform.localEulerAngles = Vector3.zero;
                prop.transform.localScale = Vector3.one;
                var launcher = prop.GetComponent<StationaryProbeLauncher>();
                if (launcher != null) {
                    var RealY = InitialDegreesY - 10;
                    launcher._verticalPivot.localEulerAngles = new Vector3(350, 0, 0);
                    launcher._photosOnly = PhotosOnly;
                    launcher._initialDegreesY = RealY;
                    launcher._lockInputY= LockInputY;
                    launcher._returnToStartPos = ReturnToStartPos;
                    launcher._interactVolume.transform.localPosition += interactPointOffset;

                    launcher._initRotX = launcher.transform.localRotation;
                    launcher._initRotY = launcher._verticalPivot.localRotation;
                    launcher._localUpAxis = launcher.transform.parent.InverseTransformDirection(launcher.transform.up);
                    launcher._degreesY = RealY;
                    launcher.transform.localRotation = Quaternion.AngleAxis(launcher._degreesX, launcher._localUpAxis) * launcher._initRotX;
                    launcher._verticalPivot.localRotation = Quaternion.AngleAxis(launcher._degreesY, -Vector3.right) * launcher._initRotY;
                    launcher._attachPoint.enabled= true;        //not sure if neccessary, but works
                    UnityUtils.DoAfterFrames(Mod, 1, () => launcher._attachPoint.enabled = false);



                }
            }
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
}
