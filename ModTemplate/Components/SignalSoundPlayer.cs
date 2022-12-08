using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class SignalSoundPlayer : WrongWarpBehaviour
    {
        public string AudioTypeName;
        public float Delay;
        public float FadeDuration;
        public bool Continuous;
        public List<Sensor> Sensors = new List<Sensor>();

        AudioSource audioSource;
        OWAudioSource owAudioSource;
        Coroutine coroutine;

        public override void WireUp()
        {

        }

        public void Awake()
        {
            audioSource = gameObject.GetAddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            owAudioSource = gameObject.GetAddComponent<OWAudioSource>();
            owAudioSource.loop = Continuous;
            owAudioSource.AssignAudioLibraryClip((AudioType)Enum.Parse(typeof(AudioType), AudioTypeName));
        }

        public void Update()
        {
            bool activated = Continuous ? Sensor.AreAllActivated(Sensors) : Sensor.WereAllActivatedThisFrame(Sensors);
            if (activated) PlaySound();
            else StopSound();
        }

        void StopSound()
        {
            owAudioSource.Stop();
        }

        void PlaySound()
        {
            if (coroutine != null && Continuous) return;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(DoPlaySoundDelayed());
        }

        IEnumerator DoPlaySoundDelayed()
        {
            yield return new WaitForSeconds(Delay);
            owAudioSource.Play();
        }
    }
}
