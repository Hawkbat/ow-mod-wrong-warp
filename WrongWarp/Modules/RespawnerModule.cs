using ModDataTools.Utilities;
using Steamworks;
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
        AssetBundle flashbackAssetBundle;

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
            if (Mod.IsInWrongWarpSystem && !Mod.SaveData.RespawnDisabled)
            {
                MakeCameraParticles(3f);
                MakeBodyParticles(3f);
                MakeShipParticles(3f);
            }
        }

        Texture2D flashbackTex;
        GameObject maskReplacementPrefab;

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

                var mask = flashback._maskTransform.Find("Props_NOM_Mask");
                mask.gameObject.SetActive(false);

                if (!maskReplacementPrefab)
                {
                    maskReplacementPrefab = Mod.SystemAssetBundle.LoadAsset<GameObject>("Assets/ModAssets/Shared/Objects/FlashbackMaskReplacement.prefab");
                    maskReplacementPrefab.SetActive(false);
                }

                var maskReplacement = UnityEngine.Object.Instantiate(maskReplacementPrefab);
                maskReplacement.transform.parent = flashback._maskTransform;
                maskReplacement.transform.localPosition = Vector3.zero;
                maskReplacement.transform.localRotation = Quaternion.identity;
                maskReplacement.layer = (int)OuterWildsLayer.Flashback;
                maskReplacement.SetActive(true);

                var snapshotDuration = 0f;
                var snapshotCount = GetFinalFlashbackImageCount();
                for (int i = 0; i < snapshotCount; i++)
                {
                    snapshotDuration += GetFinalFlashbackImageDuration();
                    flashback._imageDisplayTimes[snapshotCount - 1 - i] = snapshotDuration;
                }
                flashback._snapshotIndex = snapshotCount - 1;
                flashback._flashbackTimer = new Timer(flashback._flashbackStartDelay + flashback._playbackDelay + snapshotDuration);
            }
            Mod.StartCoroutine(DoPlayFinalFlashbackSong());
        }

        System.Collections.IEnumerator DoPlayFinalFlashbackSong()
        {
            var flashback = UnityEngine.Object.FindObjectOfType<Flashback>();
            var startDelay = flashback._flashbackStartDelay + flashback._playbackDelay;
            var finalSongClip = Mod.SystemAssetBundle.LoadAsset<AudioClip>("Assets/ModAssets/Shared/Audio/FinalFlashback.ogg");
            yield return new WaitForSeconds(startDelay);
            flashback._audioController._oneShotSource.PlayOneShot(finalSongClip);
        }

        float GetFinalFlashbackImageDuration()
        {
            var beatsPerMinute = 92f;
            var secondsPerBeat = 60f / beatsPerMinute;
            return secondsPerBeat;
        }

        int GetFinalFlashbackImageCount()
        {
            var beatsPerMeasure = 4;
            var totalMeasures = 68;
            var totalBeats = beatsPerMeasure * totalMeasures;
            var imageCount = totalBeats;
            return imageCount;
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

        public void OverwriteFlashback()
        {
            if (flashbackAssetBundle == null)
            {
                flashbackAssetBundle = Mod.ModHelper.Assets.LoadBundle("assetbundles/flashback");
            }

            var textures = flashbackAssetBundle.LoadAllAssets<Texture2D>();

            var flashback = GameObject.FindObjectOfType<Flashback>();
            var recorder = flashback._flashbackRecorder;

            var oldArray = recorder._renderTextureArray;
            var newArray = new RenderTexture[oldArray.Length];

            var oldDreamWorldArray = recorder._isSnapshotInDreamWorld;
            var newDreamWorldArray = new bool[newArray.Length];

            int oldFlashbackLength = recorder._numCapturedSnapshots;
            int newFlashbackLength = GetFinalFlashbackImageCount();

            float dstH = (PlayerData.GetGraphicSettings().textureQuality == TextureQuality.FULL) ? 270 : 135;
            float screenAspect = (float)Screen.width / Screen.height;
            float dstW = Mathf.Round(dstH * screenAspect);

            int originalStride = 2;

            int overwriteIndex = 0;
            int originalIndex = oldFlashbackLength - 1;
            int outputIndex = newArray.Length - 1;
            int sparesIndex = oldFlashbackLength;

            while (outputIndex >= 0)
            {
                if (outputIndex % originalStride == 0 || originalIndex < 0)
                {
                    RenderTexture renderTexture;
                    if (sparesIndex < oldArray.Length)
                    {
                        renderTexture = oldArray[sparesIndex++];
                    } else
                    {
                        renderTexture = new RenderTexture(Mathf.RoundToInt(dstW), Mathf.RoundToInt(dstH), 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                        renderTexture.name = "FlashbackRenderTex_" + outputIndex;
                    }

                    var texture = textures[overwriteIndex++];
                    overwriteIndex %= textures.Length;

                    newDreamWorldArray[outputIndex] = true;
                    newArray[outputIndex--] = renderTexture;

                    float srcW = texture.width;
                    float srcH = texture.height;

                    float scaleFactor = Mathf.Max(dstW / srcW, dstH / srcH);

                    Vector2 scale = Vector2.one;
                    Vector2 offset = Vector2.zero;

                    Graphics.Blit(texture, renderTexture, scale, offset);
                }
                else
                {
                    newDreamWorldArray[outputIndex] = oldDreamWorldArray[outputIndex];
                    newArray[outputIndex--] = oldArray[originalIndex--];
                }
            }

            recorder._renderTextureArray = newArray;
            recorder._isSnapshotInDreamWorld = newDreamWorldArray;
            recorder._numCapturedSnapshots = newFlashbackLength;

            flashbackAssetBundle.Unload(true);
        }
    }
}
