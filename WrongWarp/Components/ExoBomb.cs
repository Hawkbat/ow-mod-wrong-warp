using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class ExoBomb : Sensor
    {
        public OWAudioSource CountdownAudioSource;
        public List<Sensor> Sensors = new();
        public float KillRadius;
        public float HurtRadius;
        public float LaunchForce;
        public float Damage;
        public List<GameObject> ChildrenToEnable = new();
        public List<GameObject> ChildrenToDisable = new();

        ExplosionController explosion;

        bool detonating;
        bool detonated;

        public override float ComputeStrength() => detonating ? 1f : 0f;

        public override void WireUp()
        {
            var explosionGO = Instantiate(Locator.GetShipBody().GetComponent<ShipDamageController>()._explosion.gameObject);
            explosionGO.transform.parent = transform;
            explosionGO.transform.localPosition = Vector3.zero;
            explosionGO.transform.localEulerAngles = Vector3.zero;
            explosion = explosionGO.GetComponent<ExplosionController>();
        }

        public override void Update()
        {
            base.Update();
            if (Sensor.AreAllActivated(Sensors) && !detonated && !detonating)
            {
                StartCoroutine(DoDetonationSequence());
            }
            if (detonated && explosion)
            {
                explosion.enabled = true;
            }
        }

        IEnumerator DoDetonationSequence()
        {
            detonating = true;

            float delay = 1f;
            float pitch = 0.25f;
            for (int i = 0; i < 4; i++)
            {
                CountdownAudioSource.pitch = pitch;
                CountdownAudioSource.Play();
                yield return new WaitForSeconds(delay);
                CountdownAudioSource.Stop();
                delay *= 0.5f;
                pitch += 0.25f;
            }

            detonating = false;
            detonated = true;

            CountdownAudioSource.PlayOneShot(AudioType.ShipDamageShipExplosion);

            if (explosion && !explosion._playing)
                explosion.Play();

            var player = Locator.GetPlayerBody();
            var diff = (player.transform.position + Vector3.up) - transform.position;
            var dir = diff.normalized;
            var dist = diff.magnitude;
            if (dist < KillRadius)
            {
                Locator.GetDeathManager().KillPlayer(DeathType.Impact);
                player.AddImpulse(dir * LaunchForce);
            } else if (dist < HurtRadius)
            {
                if (Locator.GetPlayerSuit().IsWearingSuit())
                    player.GetComponent<PlayerResources>().ApplySuitPuncture();
                player.GetComponent<PlayerResources>().ApplyInstantDamage(Damage, InstantDamageType.Impact);
                player.AddImpulse(dir * LaunchForce);
            }

            yield return new WaitForSeconds(0.1f);

            foreach (var child in ChildrenToEnable)
            {
                child.gameObject.SetActive(true);
            }
            foreach (var child in ChildrenToDisable)
            {
                child.gameObject.SetActive(false);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, KillRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, HurtRadius);
        }
    }
}
