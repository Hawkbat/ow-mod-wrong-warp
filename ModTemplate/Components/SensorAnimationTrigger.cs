using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class SensorAnimationTrigger : WrongWarpBehaviour
    {
        public Animator Animator;
        public AnimatorControllerParameterType ParameterType;
        public string ParameterName;
        public bool Invert;
        public bool Continuous;
        public List<Sensor> Sensors = new List<Sensor>();

        public override void WireUp()
        {

        }

        public void Update()
        {
            if (!Animator) return;
            float strength = Sensor.GetMinStrength(Sensors);
            bool activated = Continuous ? Sensor.AreAllActivated(Sensors) : Sensor.WereAllActivatedThisFrame(Sensors);
            bool shouldApply = Continuous || activated;
            switch (ParameterType)
            {
                case AnimatorControllerParameterType.Bool:
                    if (shouldApply) Animator.SetBool(ParameterName, Invert ? !activated : activated);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    if (shouldApply && activated) Animator.SetTrigger(ParameterName);
                    break;
                case AnimatorControllerParameterType.Int:
                    if (shouldApply) Animator.SetInteger(ParameterName, Invert ? 1 - Mathf.RoundToInt(strength) : Mathf.RoundToInt(strength));
                    break;
                case AnimatorControllerParameterType.Float:
                    if (shouldApply) Animator.SetFloat(ParameterName, Invert ? 1f - strength : strength);
                    break;
            }
        }
    }
}
