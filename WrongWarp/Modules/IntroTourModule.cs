using OWML.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class IntroTourModule : WrongWarpModule
    {
        private OWRigidbody vesselBody;

        public IntroTourModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            DoAfterFrames(1, () =>
            {
                var vessel = GameObject.Find("Vessel_Body");
                if (vessel)
                {
                    vesselBody = vessel.GetAttachedOWRigidbody();
                    GlobalMessenger<DeathType>.AddListener("PlayerDeath", OnDiedInIntroTour);
                    Mod.SaveData[SaveDataFlag.HasDoneIntroTour] = false;
                    DoAfterSeconds(0.1f, () => {
                        StartSequence();
                    });
                }
            });
        }

        public override void OnSystemUnload()
        {
            vesselBody = null;
        }

        public override void OnFixedUpdate()
        {
            if (vesselBody)
            {
                vesselBody.AddAcceleration(Mod.TweakConfig.introTour.vessel.acceleration * Time.fixedDeltaTime);
                vesselBody.AddAngularAcceleration(Mod.TweakConfig.introTour.vessel.angularAcceleration * Time.fixedDeltaTime);
            }
        }

        private void OnDiedInIntroTour(DeathType deathType)
        {
            Mod.SaveData[SaveDataFlag.HasDoneIntroTour] = true;
            GlobalMessenger<DeathType>.RemoveListener("PlayerDeath", OnDiedInIntroTour);
        }

        private void StartSequence()
        {
            var corePlanet = Mod.NewHorizonsApi.GetPlanet("WW_CORE");
            var shipBody = Locator.GetShipBody();
            shipBody.SetPosition(corePlanet.GetAttachedOWRigidbody().GetPosition());

            Locator.GetPlayerSuit().SuitUp(false, true, true);

            Locator.GetPlayerCameraController().StopSnapping();

            UnityUtils.GetTransformAtPath(vesselBody.transform, "./Sector_VesselBridge/Volumes_VesselBridge/GravityOxygenVolume_VesselBridge").gameObject.SetActive(false);

            var alarmObj = new GameObject("Alarm");
            alarmObj.transform.parent = UnityUtils.GetTransformAtPath(vesselBody.transform, "./Sector_VesselBridge/Interactibles_VesselBridge/WarpCoreSocket");
            alarmObj.transform.localPosition = Vector3.zero;
            var alarm = alarmObj.AddComponent<OWAudioSource>();
            alarm.SetTrack(OWAudioMixer.TrackName.Environment);
            alarm.AssignAudioLibraryClip(AudioType.NomaiEscapePodDistressSignal_LP);
            alarm.loop = true;
            alarm.Play();

            var airLeakObj = new GameObject("Air Leak");
            airLeakObj.transform.parent = UnityUtils.GetTransformAtPath(vesselBody.transform, "./Sector_VesselBridge/Interactibles_VesselBridge/CoordinateInterface");
            airLeakObj.transform.localPosition = Vector3.zero;
            var airLeak = airLeakObj.AddComponent<OWAudioSource>();
            airLeak.SetTrack(OWAudioMixer.TrackName.Environment);
            airLeak.AssignAudioLibraryClip(AudioType.AirRushingOut);
            airLeak.loop = true;
            airLeak.Play();

            Locator.GetPlayerCamera().GetComponent<PlayerCameraEffectController>().OpenEyes(1f, false);

            var plrRes = Locator.GetPlayerBody().GetComponentInChildren<PlayerResources>();
            plrRes.ApplySuitPuncture();
            plrRes._currentFuel = 10f;
            plrRes._currentHealth = 50f;

            TimeLoop.SetSecondsRemaining(60f);
        }
    }
}
