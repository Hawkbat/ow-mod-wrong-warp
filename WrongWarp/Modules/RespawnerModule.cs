﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Components;
using WrongWarp.Utils;

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
                DoAfterSeconds(0.1f, () =>
                {
                    RespawnPlayer();
                    RespawnShip();
                });
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
                MakeShipParticles(2f);
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
            var spawnPoint = Mod.NewHorizonsApi.GetPlanet("WW_HEARTHIAN_EXHIBIT")
                .GetComponentsInChildren<SpawnPoint>()
                .FirstOrDefault(s => !s.IsShipSpawn());
            if (spawnPoint != null)
            {
                var spawner = Locator.GetPlayerBody().GetComponent<PlayerSpawner>();
                spawner.DebugWarp(spawnPoint);
                MakeCameraParticles(1f);
                MakeBodyParticles(1f);
            }
        }

        public void RespawnShip()
        {
            var spawnPoint = Mod.NewHorizonsApi.GetPlanet("WW_HEARTHIAN_EXHIBIT")
                .GetComponentsInChildren<SpawnPoint>()
                .First(s => s.IsShipSpawn());

            var shipBody = Locator.GetShipBody();

            var shipDamageCtrl = shipBody.GetComponent<ShipDamageController>();
            shipDamageCtrl._invincible = true;

            MakeShipParticles(1f);

            for (int i = 0; i < 10; i++)
            {
                UnityUtils.DoAfterSeconds(Mod, 0.2f * i, () =>
                {
                    shipBody.WarpToPositionRotation(spawnPoint.transform.position + spawnPoint.transform.up * 4f, spawnPoint.transform.rotation);
                    shipBody.SetVelocity(spawnPoint.GetAttachedOWRigidbody().GetPointVelocity(spawnPoint.transform.position));
                });
            }
            UnityUtils.DoAfterSeconds(Mod, 0.2f * 10f, () =>
            {
                foreach (var hull in shipDamageCtrl._shipHulls)
                    while (hull._damaged) hull.RepairTick();
                foreach (var comp in shipDamageCtrl._shipComponents)
                    while (comp._damaged) comp.RepairTick();
                shipDamageCtrl._invincible = false;
            });
        }

        GameObject playerSpawnEffectPrefab;
        GameObject cameraSpawnEffectPrefab;
        GameObject shipSpawnEffectPrefab;

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

        public void MakeShipParticles(float duration)
        {
            if (!shipSpawnEffectPrefab)
            {
                shipSpawnEffectPrefab = Mod.SystemAssetBundle.LoadAsset<GameObject>("Assets/ModAssets/Shared/Objects/ShipSpawnEffect.prefab");
                shipSpawnEffectPrefab.SetActive(false);
            }

            var shipBody = Locator.GetShipBody();
            var shipSpawnEffect = GameObject.Instantiate(shipSpawnEffectPrefab, shipBody.transform, false);
            var teleporterFx = shipSpawnEffect.GetComponent<TeleporterEffect>();
            teleporterFx.Duration = duration;
            shipSpawnEffect.SetActive(true);
        }
    }
}
