using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Components;

namespace WrongWarp.Modules
{
    public class RespawnerModule : WrongWarpModule
    {
        private GameObject shipBackup;

        public RespawnerModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            GlobalMessenger<DeathType>.AddListener("PlayerDeath", OnPlayerDeath);
            GlobalMessenger.AddListener("FlashbackStart", FlashbackStart);
            if (Mod.SaveData.HasDoneIntroTour)
            {
                DoAfterSeconds(0.1f, () => SpawnAtMuseum());
            }
        }

        public override void OnSystemUnload()
        {
            GlobalMessenger<DeathType>.RemoveListener("PlayerDeath", OnPlayerDeath);
            GlobalMessenger.RemoveListener("FlashbackStart", FlashbackStart);
        }

        private void OnPlayerDeath(DeathType deathType)
        {
            if (Mod.IsInWrongWarpSystem)
            {
                MakeCameraParticles(2f);
                MakeBodyParticles(2f);
            }
        }

        Texture2D flashbackTex;

        private void FlashbackStart()
        {
            if (Mod.IsInWrongWarpSystem)
            {
                if (!flashbackTex)
                {
                    flashbackTex = Mod.SystemAssetBundle.LoadAsset<Texture2D>("Assets/ModAssets/Shared/Textures/FlashbackStreams.png");
                    flashbackTex.wrapMode = TextureWrapMode.Clamp;
                    flashbackTex.wrapModeU = TextureWrapMode.Clamp;
                    flashbackTex.wrapModeV = TextureWrapMode.Repeat;
                    flashbackTex.wrapModeW = TextureWrapMode.Repeat;
                }

                var flashback = UnityEngine.Object.FindObjectOfType<Flashback>();
                flashback._matPropBlock_Streams.SetColor("_EmissionColor", Mod.TweakConfig.flashbackColor);
                flashback._matPropBlock_Streams.SetTexture("_MainTex", flashbackTex);

                flashback._maskTransform.gameObject.SetActive(false);
            }
        }

        public void RespawnPlayer()
        {
            var spawnPoint = Mod.NewHorizonsApi.GetPlanet("Hearthian Exhibit")
                .GetComponentsInChildren<SpawnPoint>()
                .FirstOrDefault(s => !s.IsShipSpawn());
            if (spawnPoint != null)
            {
                var spawner = Locator.GetPlayerBody().GetComponent<PlayerSpawner>();
                spawner.DebugWarp(spawnPoint);
                SpawnAtMuseum();
            }
        }

        public void RespawnShip()
        {
            var spawnPoint = Mod.NewHorizonsApi.GetPlanet("Hearthian Exhibit")
                .GetComponentsInChildren<SpawnPoint>()
                .First(s => s.IsShipSpawn());
            var shipDamageCtrl = Locator.GetShipBody().GetComponent<ShipDamageController>();
            if (shipDamageCtrl.IsHullBreached())
            {
                GameObject.Destroy(Locator.GetShipBody().gameObject);
                var freshShip = GameObject.Instantiate(shipBackup);

                freshShip.transform.position = spawnPoint.transform.position;
                freshShip.transform.rotation = spawnPoint.transform.rotation;

                freshShip.SetActive(true);

                Locator.GetPlayerCameraController()._shipController = freshShip.GetComponentInChildren<ShipCockpitController>();
                Locator.GetToolModeSwapper()._shipSystemsCtrlr = freshShip.GetComponentInChildren<ShipCockpitController>();
                GameObject.FindObjectOfType<ToolModeUI>()._landingManager = freshShip.GetComponentInChildren<LandingPadManager>();
                freshShip.GetComponentInChildren<ShipCockpitController>().enabled = true;

                DoAfterFrames(1, () =>
                {
                    freshShip.GetComponent<OWRigidbody>().WarpToPositionRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);
                    freshShip.GetComponent<OWRigidbody>().SetVelocity(spawnPoint.GetAttachedOWRigidbody().GetPointVelocity(spawnPoint.transform.position));
                });

                Locator._shipTransform = freshShip.GetComponent<Transform>();
                Locator._shipBody = freshShip.GetComponent<OWRigidbody>();
                Locator._shipDetector = freshShip.transform.Find("ShipDetector").gameObject;

                freshShip.GetComponent<ShipDamageController>()._invincible = true;
                DoAfterSeconds(1f, () =>
                {
                    freshShip.GetComponent<ShipDamageController>()._invincible = false;
                });
            } else
            {
                foreach (var hull in shipDamageCtrl._shipHulls)
                    while (hull._damaged) hull.RepairTick();
                foreach (var comp in shipDamageCtrl._shipComponents)
                    while (comp._damaged) comp.RepairTick();

                var shipBody = Locator.GetShipBody();
                shipBody.WarpToPositionRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);
                shipBody.SetVelocity(spawnPoint.GetAttachedOWRigidbody().GetPointVelocity(spawnPoint.transform.position));
            }
        }

        private void SpawnAtMuseum()
        {
            if (!shipBackup)
            {
                var ship = Locator.GetShipBody().gameObject;
                ship.SetActive(false);
                shipBackup = GameObject.Instantiate(ship);
                shipBackup.name = ship.name;
                ship.SetActive(true);
            }
            MakeCameraParticles(1f);
            MakeBodyParticles(1f);
        }

        GameObject playerSpawnEffectPrefab;
        GameObject cameraSpawnEffectPrefab;

        public void MakeBodyParticles(float duration)
        {
            if (!playerSpawnEffectPrefab)
            {
                playerSpawnEffectPrefab = Mod.SystemAssetBundle.LoadAsset<GameObject>("Assets/ModAssets/Shared/Objects/PlayerSpawnEffect.prefab");
                playerSpawnEffectPrefab.SetActive(false);
            }

            var playerBody = Locator.GetPlayerBody();
            var playerSpawnEffect = GameObject.Instantiate(playerSpawnEffectPrefab, playerBody.transform, false);
            var teleporterFx = playerSpawnEffect.GetComponent<TeleporterEffect>();
            teleporterFx.Duration = duration;
            playerSpawnEffect.SetActive(true);
        }

        public void MakeCameraParticles(float duration)
        {
            if (!cameraSpawnEffectPrefab)
            {
                cameraSpawnEffectPrefab = Mod.SystemAssetBundle.LoadAsset<GameObject>("Assets/ModAssets/Shared/Objects/CameraSpawnEffect.prefab");
                cameraSpawnEffectPrefab.SetActive(false);
            }

            var playerCamera = Locator.GetPlayerCamera().transform;
            var cameraSpawnEffect = GameObject.Instantiate(cameraSpawnEffectPrefab, playerCamera.transform, false);
            var teleporterFx = cameraSpawnEffect.GetComponent<TeleporterEffect>();
            teleporterFx.Duration = duration;
            cameraSpawnEffect.SetActive(true);
        }
    }
}
