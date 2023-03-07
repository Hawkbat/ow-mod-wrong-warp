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
                var vesselWarpController = GameObject.FindObjectOfType<VesselWarpController>();
                if (vesselWarpController && vesselWarpController.isActiveAndEnabled)
                {
                    GlobalMessenger<DeathType>.AddListener("PlayerDeath", OnDiedInIntroTour);
                    Mod.SaveData.HasDoneIntroTour = false;
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
            Mod.SaveData.HasDoneIntroTour = true;
            GlobalMessenger<DeathType>.RemoveListener("PlayerDeath", OnDiedInIntroTour);
        }

        private void StartSequence()
        {
            var corePlanet = Mod.NewHorizonsApi.GetPlanet("Core");
            var shipBody = Locator.GetShipBody();
            shipBody.SetPosition(corePlanet.GetAttachedOWRigidbody().GetPosition());

            Locator.GetPlayerSuit().SuitUp(false, true, true);

            Locator.GetPlayerCameraController().StopSnapping();

            vesselBody = GameObject.FindObjectOfType<VesselWarpController>().GetAttachedOWRigidbody();
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

            UnityUtils.DoAfterSeconds(Mod, 0f, () =>
            {
                NotificationManager.SharedInstance.PostNotification(new NotificationData(NotificationTarget.Player, "SUIT INTEGRITY FAILURE"), true);
            });
            UnityUtils.DoAfterSeconds(Mod, 5f, () =>
            {
                NotificationManager.SharedInstance.PostNotification(new NotificationData(NotificationTarget.Player, "TEMPERATURE LEVELS RISING"), true);
            });
            UnityUtils.DoAfterSeconds(Mod, 10f, () =>
            {
                NotificationManager.SharedInstance.PostNotification(new NotificationData(NotificationTarget.Player, "TEMPERATURE LEVELS CRITICAL"), true);

                var burnObj = new GameObject("Burn");
                burnObj.SetActive(false);
                burnObj.transform.parent = Locator.GetPlayerBody().transform;
                burnObj.transform.localPosition = Vector3.zero;
                burnObj.layer = LayerMask.NameToLayer("BasicEffectVolume");
                var burnShape = burnObj.AddComponent<SphereShape>();
                burnShape._collisionMode = Shape.CollisionMode.Volume;
                burnShape._radius = 1000f;
                var burnTrigger = burnObj.AddComponent<OWTriggerVolume>();
                var burnHazard = burnObj.AddComponent<HeatHazardVolume>();
                burnHazard._damagePerSecond = 2f;
                burnHazard._triggerVolume = burnTrigger;
                burnObj.SetActive(true);
                burnTrigger.AddObjectToVolume(Locator.GetPlayerDetector().gameObject);
                burnTrigger.AddObjectToVolume(Locator.GetPlayerCamera().GetComponentInChildren<FluidDetector>().gameObject);
            });

            //DoAfterSeconds(Mod.TweakConfig.introTour.supernovaTime, () => Locator.GetPlayerBody().GetComponentInChildren<DeathManager>().KillPlayer(DeathType.Supernova));
        }
    }
}
