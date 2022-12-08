using OWML.Common;
using System;
using System.Collections.Generic;
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
            DoAfterSeconds(0.1f, () => {
                if (!Mod.SaveData.HasDoneIntroTour)
                {
                    Mod.SaveData.HasDoneIntroTour = true;
                    StartSequence();
                }
            });
        }

        public override void OnSystemUnload()
        {
            CleanupVessel();
        }

        public override void OnFixedUpdate()
        {
            if (vesselBody)
            {
                vesselBody.AddAcceleration(Mod.TweakConfig.introTour.vessel.acceleration * Time.fixedDeltaTime);
                vesselBody.AddAngularAcceleration(Mod.TweakConfig.introTour.vessel.angularAcceleration * Time.fixedDeltaTime);
            }
        }

        private void CleanupVessel()
        {
            vesselBody = null;
        }

        private void StartSequence()
        {
            var shipBody = Locator.GetShipBody();
            shipBody.SetPosition(Vector3.zero);

            Locator.GetPlayerSuit().SuitUp(false, true, true);

            /*
            Mod.ModHelper.Console.WriteLine($"Spawning vessel", MessageType.Info);
            var planet = Mod.NewHorizonsApi.GetPlanet("Core");
            var clone = Mod.NewHorizonsApi.SpawnObject(
                planet,
                planet.gameObject.GetComponentInChildren<Sector>(),
                "DB_VesselDimension_Body/Sector_VesselDimension",
                Mod.TweakConfig.introTour.vessel.position,
                Mod.TweakConfig.introTour.vessel.rotation,
                1f,
                false);
            var vessel = clone;
            Mod.ModHelper.Console.WriteLine($"Setting up vessel", MessageType.Info);
            vessel.transform.Find("Vessel_Atmosphere").gameObject.SetActive(false);
            vessel.transform.Find("Geometry_VesselDimension/OtherComponentsGroup/Vines").gameObject.SetActive(false);
            vessel.transform.Find("Geometry_VesselDimension/OtherComponentsGroup/Terrain_DB_BrambleSphere_Outer_v2").gameObject.SetActive(false);
            vessel.transform.Find("Geometry_VesselDimension/BatchedGroup/BatchedMeshColliders_1").gameObject.SetActive(false);
            vessel.transform.Find("Geometry_VesselDimension/BatchedGroup/BatchedMeshColliders_2").gameObject.SetActive(false);
            vessel.transform.Find("Geometry_VesselDimension/OtherComponentsGroup/Structure_NOM_Vessel/glass").gameObject.SetActive(false);
            foreach (Transform t in vessel.transform.Find("Interactables_VesselDimension/DynamicProps_VesselDimension")) t.gameObject.SetActive(false);
            foreach (Transform t in vessel.transform.Find("Sector_VesselBridge/Interactibles_VesselBridge/DynamicProps")) t.gameObject.SetActive(false);
            Mod.ModHelper.Console.WriteLine($"Adding vessel rigidbody", MessageType.Info);
            vesselBody = vessel.gameObject.AddComponent<OWRigidbody>();
            vesselBody.MakeKinematic();
            vesselBody.EnableKinematicSimulation();
            Mod.ModHelper.Console.WriteLine($"Enabling warp controller", MessageType.Info);
            var ctrl = vessel.GetComponentInChildren<VesselWarpController>();
            ctrl.SetPowered(true);
            Mod.ModHelper.Console.WriteLine($"Adding vessel alarm", MessageType.Info);
            var alarm = vessel.AddComponent<OWAudioSource>();
            alarm.SetTrack(OWAudioMixer.TrackName.Environment);
            alarm.AssignAudioLibraryClip(AudioType.NomaiEscapePodDistressSignal_LP);
            alarm.loop = true;
            alarm.Play();

            SpawnAtVessel();
            */

            DoAfterSeconds(Mod.TweakConfig.introTour.supernovaTime, () => GlobalMessenger.FireEvent("TriggerSupernova"));
        }
        /*
        private void SpawnAtVessel(int retryCount = 10)
        {
            var playerBody = Locator.GetPlayerBody();
            var planet = Mod.NewHorizonsApi.GetPlanet("Core");
            vesselBody.SetVelocity(Vector3.zero);
            playerBody.SetVelocity(Vector3.zero);
            playerBody.WarpToPositionRotation(
                planet.transform.TransformPoint(
                    Mod.TweakConfig.introTour.vessel.position + Mod.TweakConfig.introTour.player.offset),
                Quaternion.Euler(Mod.TweakConfig.introTour.player.rotation));
            if (retryCount > 0) DoAfterSeconds(0.1f, () => SpawnAtVessel(retryCount - 1));
        }
        */
    }
}
