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
        public TumbleLock Master;

        public override void WireUp()
        {
            Mod.TumbleLocks.Register(this);
            if (Master)
            {
                Frequency = Master.Frequency;
                Rotations = Master.Rotations;
                RotationIndex = Master.RotationIndex;
                RotationSpeed = Master.RotationSpeed;
            }
        }

        void OnDisable()
        {
            Mod.TumbleLocks.Unregister(this);
        }

        void Update()
        {
            if (Master)
            {
                RotationIndex = Master.RotationIndex;
            }
            var curRot = transform.localRotation;
            var targetRot = Quaternion.Euler(0f, Rotations[RotationIndex], 0f);
            var rotSpeed = RotationSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.RotateTowards(curRot, targetRot, rotSpeed);
        }

        public bool IsAtTarget(int index)
        {
            var curRot = transform.localRotation;
            var targetRot = Quaternion.Euler(0f, Rotations[index], 0f);
            return Quaternion.Angle(curRot, targetRot) < 0.1f;
        }
    }
}
