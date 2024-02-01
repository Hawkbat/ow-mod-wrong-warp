using ModDataTools.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class TumbleLock : WrongWarpBehaviour
    {
        public FrequencyAsset Frequency;
        public List<float> Rotations = new();
        public int RotationIndex;
        public float RotationSpeed;

        public override void WireUp()
        {
            Mod.TumbleLocks.Register(this);
        }

        void OnDisable()
        {
            Mod.TumbleLocks.Unregister(this);
        }

        void Update()
        {
            var curRot = transform.localRotation;
            var targetRot = Quaternion.Euler(0f, Rotations[RotationIndex], 0f);
            var rotSpeed = RotationSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.RotateTowards(curRot, targetRot, rotSpeed);
        }
    }
}
