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

        private void SpawnAtMuseum(int retryCount = 10)
        {
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
