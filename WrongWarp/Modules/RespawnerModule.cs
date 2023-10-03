using System;
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
        bool isRespawningPlayer;
        bool isRespawningShip;

        public bool IsRespawningPlayer => isRespawningPlayer;
        public bool IsRespawningShip => isRespawningShip;

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
                MakeCameraParticles(3f);
                MakeBodyParticles(3f);
                MakeShipParticles(3f);
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
            if (isRespawningPlayer) return;
            isRespawningPlayer = true;
            var spawnPoint = Mod.NewHorizonsApi.GetPlanet("WW_HEARTHIAN_EXHIBIT")
                .GetComponentsInChildren<SpawnPoint>(true)
                .FirstOrDefault(s => !s.IsShipSpawn());
            if (spawnPoint != null)
            {
                MakeCameraParticles(1f);
                MakeBodyParticles(1f);
                UnityUtils.DoAfterSeconds(Mod, 1f, () =>
                {
                    var spawner = Locator.GetPlayerBody().GetComponent<PlayerSpawner>();
                    spawner.DebugWarp(spawnPoint);
                    isRespawningPlayer = false;
                });
            }
        }

        public void RespawnShip()
        {
            if (isRespawningShip) return;
            isRespawningShip = true;

            SpawnPoint spawnPoint;
            if (Mod.SaveData.ShipSpawnChanged)
            {
                spawnPoint = Mod.NewHorizonsApi.GetPlanet("WW_SEEKERS_EXHIBIT")
                    .GetComponentsInChildren<SpawnPoint>(true)
                    .First(s => s.IsShipSpawn());
            } else
            {
                spawnPoint = Mod.NewHorizonsApi.GetPlanet("WW_HEARTHIAN_EXHIBIT")
                    .GetComponentsInChildren<SpawnPoint>(true)
                    .First(s => s.IsShipSpawn());
            }

            var shipBody = Locator.GetShipBody();

            var shipDamageCtrl = shipBody.GetComponent<ShipDamageController>();
            shipDamageCtrl._invincible = true;

            MakeShipParticles(1f);
            var spawnParticles = MakeShipParticles(1f, spawnPoint.transform);

            UnityUtils.DoAfterSeconds(Mod, 1f, () =>
            {
                spawnParticles.transform.parent = null;
                spawnParticles.gameObject.AddComponent<Rigidbody>();
                var spawnRB = spawnParticles.gameObject.AddComponent<OWRigidbody>();
                var spawnCUO = spawnParticles.gameObject.AddComponent<CenterOfTheUniverseOffsetApplier>();
                spawnCUO._body = spawnRB;

                spawnRB.WarpToPositionRotation(shipBody.transform.position, shipBody.transform.rotation);
                spawnRB.SetVelocity(shipBody.GetVelocity());
                
                for (int i = 0; i < 10; i++)
                {
                    UnityUtils.DoAfterSeconds(Mod, 0.1f * i, () =>
                    {
                        shipBody.WarpToPositionRotation(spawnPoint.transform.position + spawnPoint.transform.up * 4f, spawnPoint.transform.rotation);
                        shipBody.SetVelocity(spawnPoint.GetAttachedOWRigidbody().GetVelocity());
                    });
                }
                UnityUtils.DoAfterSeconds(Mod, 0.1f * 10f, () =>
                {
                    foreach (var hull in shipDamageCtrl._shipHulls)
                        while (hull._damaged) hull.RepairTick();
                    foreach (var comp in shipDamageCtrl._shipComponents)
                        while (comp._damaged) comp.RepairTick();
                    Locator.GetShipBody().GetComponentInChildren<ShipResources>().RefillResources();

                    shipDamageCtrl._invincible = false;
                    isRespawningShip = false;
                });
            });

        }

        GameObject playerSpawnEffectPrefab;
        GameObject cameraSpawnEffectPrefab;
        GameObject shipSpawnEffectPrefab;

        public TeleporterEffect MakeBodyParticles(float duration)
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

            return teleporterFx;
        }

        public TeleporterEffect MakeCameraParticles(float duration)
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

            return teleporterFx;
        }

        public void MakeShipParticles(float duration)
            => MakeShipParticles(duration, Locator.GetShipBody().transform);

        public TeleporterEffect MakeShipParticles(float duration, Transform target)
        {
            if (!shipSpawnEffectPrefab)
            {
                shipSpawnEffectPrefab = Mod.SystemAssetBundle.LoadAsset<GameObject>("Assets/ModAssets/Shared/Objects/ShipSpawnEffect.prefab");
                shipSpawnEffectPrefab.SetActive(false);
            }

            var shipSpawnEffect = GameObject.Instantiate(shipSpawnEffectPrefab, target, false);
            var teleporterFx = shipSpawnEffect.GetComponent<TeleporterEffect>();
            teleporterFx.Duration = duration;
            shipSpawnEffect.SetActive(true);

            return teleporterFx;
        }
    }
}
