using UnityEngine;

namespace WrongWarp.Components
{
    public class TriggerVolumeSensor : Sensor
    {
        public bool AllowPlayer = true;
        public bool AllowScout;
        public bool AllowShip;

        OWTriggerVolume triggerVolume;
        int occupantCount = 0;

        public override float ComputeStrength() => occupantCount > 0 ? 1f : 0f;

        public override void WireUp()
        {

        }

        protected void Awake()
        {
            triggerVolume = GetComponentInChildren<OWTriggerVolume>(true);
            triggerVolume.OnEntry += TriggerVolume_OnEntry;
            triggerVolume.OnExit += TriggerVolume_OnExit;
        }

        protected void OnDestroy()
        {
            triggerVolume.OnEntry -= TriggerVolume_OnEntry;
            triggerVolume.OnExit -= TriggerVolume_OnExit;
        }

        void TriggerVolume_OnEntry(GameObject hitObj)
        {
            if (AllowPlayer && hitObj.CompareTag("PlayerDetector"))
            {
                occupantCount++;
            }
            else if (AllowScout && hitObj.CompareTag("ProbeDetector"))
            {
                occupantCount++;
            }
            else if (AllowShip && hitObj.CompareTag("ShipDetector"))
            {
                occupantCount++;
            }
        }

        void TriggerVolume_OnExit(GameObject hitObj)
        {
            if (AllowPlayer && hitObj.CompareTag("PlayerDetector"))
            {
                occupantCount = Mathf.Max(0, occupantCount - 1);
            }
            else if (AllowScout && hitObj.CompareTag("ProbeDetector"))
            {
                occupantCount = Mathf.Max(0, occupantCount - 1);
            }
            else if (AllowShip && hitObj.CompareTag("ShipDetector"))
            {
                occupantCount = Mathf.Max(0, occupantCount - 1);
            }
        }
    }
}
