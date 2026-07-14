using UnityEngine;

namespace WrongWarp.Components
{
    public abstract class RadialSensor : Sensor
    {
        public float MinDistance;
        public float MaxDistance;

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, MinDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, MaxDistance);
        }
    }
}
