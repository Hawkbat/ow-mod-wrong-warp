using UnityEngine;

namespace WrongWarp.Components
{
    public class HoloOrb : WrongWarpBehaviour
    {
        public Sensor ActivationSensor;
        public Sensor PulseSensor;
        public float PulseSpeed;
        public float PulseAmount;
        public Transform Orb;

        MeshRenderer renderer;
        Material originalMaterial;
        Material material;

        protected void Awake()
        {
            renderer = GetComponentInChildren<MeshRenderer>();
            originalMaterial = renderer.sharedMaterial;
        }

        public override void WireUp()
        {

        }

        protected void Update()
        {
            var shouldBeActive = !ActivationSensor || Sensor.IsActivated(ActivationSensor);

            if (Sensor.WasActivatedThisFrame(PulseSensor))
            {
                material = new Material(originalMaterial);
                material.SetFloat("_AlphaCutoff", 0f);
                renderer.sharedMaterial = material;
            }
            if (shouldBeActive && Sensor.IsActivated(PulseSensor))
            {
                Orb.localScale = Vector3.one * (1f + PulseAmount * Mathf.Sin(Time.time * Mathf.PI * 2f * PulseSpeed));
            }
            else
            {
                Orb.localScale = Vector3.MoveTowards(Orb.localScale, shouldBeActive ? Vector3.one : Vector3.zero, Time.deltaTime * 5f);
            }
            if (Sensor.WasDeactivatedThisFrame(PulseSensor))
            {
                renderer.sharedMaterial = originalMaterial;
                Destroy(material);
                material = null;
            }
        }
    }
}
