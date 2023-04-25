using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModDataTools.Utilities;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.InputSystem;
using WrongWarp.Components;
using WrongWarp.Configs;
using WrongWarp.Modules;
using WrongWarp.NewHorizons;
using WrongWarp.Utils;

namespace WrongWarp
{
    public class WrongWarpMod : ModBehaviour
    {
        public static WrongWarpMod Instance;

        public const string SOLAR_SYSTEM_NAME = "Hawkbar.WRONG_WARP";
        public const string TWEAK_CONFIG_PATH = "tweak.json";

        public bool IsInWrongWarpSystem;

        public INewHorizonsApi NewHorizonsApi;
        public TweakConfig TweakConfig;
        public List<WrongWarpModule> Modules;
        public PatchModule Patches;
        public SaveDataModule SaveData;
        public CuratorModule Curator;
        public DebugModeModule Debug;
        public DeviceSignalModule DeviceSignals;
        public ExoCorpseModule ExoCorpses;
        public EyeSequenceModule EyeSequence;
        public HeatModule Heat;
        public IntroTourModule IntroTour;
        public MuseumModule Museum;
        public QuantumEntityModule QuantumEntities;
        public RespawnerModule Respawner;
        public SignalTowerModule SignalTowers;
        public WarpModule Warp;
        public WormholeModule Wormhole;
        public int SystemLoadCounter;
        public AssetBundle SystemAssetBundle;

        private bool wasSystemChange;

        private void Start()
        {
            Instance = this;

            ModHelper.Console.WriteLine($"Loading {nameof(WrongWarp)}...", MessageType.Info);

            ReloadTweakConfig();

            SystemAssetBundle = ModHelper.Assets.LoadBundle("assetbundles/shared");

            Modules = new List<WrongWarpModule>();
            Patches = new PatchModule(this);
            SaveData = new SaveDataModule(this);
            Curator = new CuratorModule(this);
            Debug = new DebugModeModule(this);
            DeviceSignals = new DeviceSignalModule(this);
            ExoCorpses = new ExoCorpseModule(this);
            EyeSequence = new EyeSequenceModule(this);
            Heat = new HeatModule(this);
            IntroTour = new IntroTourModule(this);
            Museum = new MuseumModule(this);
            QuantumEntities = new QuantumEntityModule(this);
            Respawner = new RespawnerModule(this);
            SignalTowers = new SignalTowerModule(this);
            Warp = new WarpModule(this);
            Wormhole = new WormholeModule(this);

            NewHorizonsApi = ModHelper.Interaction.TryGetModApi<INewHorizonsApi>("xen.NewHorizons");
            NewHorizonsApi.LoadConfigs(this);

            NewHorizonsApi.GetChangeStarSystemEvent().AddListener(OnChangeStarSystem);
            NewHorizonsApi.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);

            ModHelper.Console.WriteLine($"{nameof(WrongWarp)} is loaded!", MessageType.Success);
        }

        private void OnChangeStarSystem(string system)
        {
            ModHelper.Console.WriteLine($"Changing to star system {system}", MessageType.Info);
            ReloadTweakConfig();
            foreach (var module in Modules)
            {
                if (module.Active)
                    module.OnSystemUnload();
            }
            IsInWrongWarpSystem = false;
            wasSystemChange = true;
        }

        private void OnStarSystemLoaded(string system)
        {
            ModHelper.Console.WriteLine($"Loaded star system {system}", MessageType.Info);
            if (system == SOLAR_SYSTEM_NAME)
            {
                IsInWrongWarpSystem = true;
                SaveData.WrongWarpTaken = true;
                SystemLoadCounter++;
                SetupNewHorizonsConfigObjectTypes();
                foreach (var module in Modules) module.OnSystemLoad();

                UnityUtils.DoAfterFrames(this, 2, () =>
                {
                    var playerCam = Locator.GetPlayerCamera();
                    if (playerCam && playerCam.mainCamera)
                        playerCam.mainCamera.depthTextureMode = DepthTextureMode.Depth;

                    foreach (var planet in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Where(o => o.name.StartsWith("WW_COREAsteroid")))
                    {
                        var icosphere = UnityUtils.GetTransformAtPath(planet.transform, "./Sector/Icosphere");
                        var vmat = icosphere.gameObject.AddComponent<VanillaMaterial>();
                        vmat.Type = VanillaMaterial.MaterialType.QuantumRock;
                    }
                });
            } else if (wasSystemChange)
            {
                IsInWrongWarpSystem = false;
                SaveData.WrongWarpTaken = false;
            } else
            {
                IsInWrongWarpSystem = false;
                if (SaveData.WrongWarpTaken)
                {
                    Warp.WarpToWrongWarpSystem();
                }
            }

            wasSystemChange = false;

            foreach (var module in Modules)
            {
                if (module.Active)
                    module.OnSystemLoad();
            }
        }

        private void ReloadTweakConfig()
        {
            TweakConfig = JsonUtils.GetOrCreate<TweakConfig>(this, TWEAK_CONFIG_PATH);
        }

        public void Update()
        {
            foreach (var module in Modules.Where(m => m.Active))
                module.OnUpdate();
        }

        public void LateUpdate()
        {
            foreach (var module in Modules.Where(m => m.Active))
                module.OnLateUpdate();
        }

        public void FixedUpdate()
        {
            foreach (var module in Modules.Where(m => m.Active))
                module.OnFixedUpdate();
        }

        public void SetupNewHorizonsConfigObjectTypes()
        {
            var planetFolderPath = $"{ModHelper.Manifest.ModFolderPath}/planets/{SOLAR_SYSTEM_NAME}/";
            var planetFileFullPaths = Directory.EnumerateFiles(planetFolderPath);
            foreach (var planetFileFullPath in planetFileFullPaths)
            {
                try
                {
                    var planetFileName = Path.GetFileName(planetFileFullPath);
                    var planetFilePath = $"planets/{SOLAR_SYSTEM_NAME}/{planetFileName}";
                    var config = ModHelper.Storage.Load<NHPlanetConfig>(planetFilePath);
                    if (config == null || config.Props == null || config.Props.details == null) continue;
                    var planet = NewHorizonsApi.GetPlanet(config.name);
                    if (planet == null) continue;
                    var detailNames = config.Props.details
                        .Select(d => d.rename ?? d.path.Split('/').Last())
                        .ToList();
                    for (var i = 0; i < config.Props.details.Count; i++)
                    {
                        var detail = config.Props.details[i];
                        if (detail.modObjectType == null && detail.modObjectTypeNames == null) continue;
                        var detailName = detailNames[i];
                        var matchingNames = i > 0 ? detailNames.Take(i).Where(n => n == detailName).Count() : 0;
                        var matchingTransform = UnityUtils.GetChildren(planet.transform.Find("Sector"))
                            .Where(t => t.name == detailName)
                            .Skip(matchingNames)
                            .FirstOrDefault();
                        if (matchingTransform)
                        {
                            if (detail.modObjectType != null)
                                ApplyModObjectType(matchingTransform.gameObject, null, detail.modObjectType);
                            if (detail.modObjectTypeNames != null)
                            {
                                foreach (var objectTypeName in detail.modObjectTypeNames)
                                {
                                    ApplyModObjectType(matchingTransform.gameObject, objectTypeName);
                                }
                            }
                        }
                    }
                } catch (Exception ex)
                {
                    ModHelper.Console.WriteLine(ex.ToString(), MessageType.Error);
                }
            }
        }

        public void ApplyModObjectType(GameObject go, string type)
        {
            if (TweakConfig.objectTypes.TryGetValue(type, out ObjectTypeConfig config))
            {
                ApplyModObjectType(go, type, config);
            }
        }

        private void ApplyModObjectType(GameObject go, string type, ObjectTypeConfig config)
        {
            var mo = go.GetComponent<WrongWarpObject>();
            if (!mo)
            {
                mo = go.AddComponent<WrongWarpObject>();
                mo.Mod = this;
            }
            if (!string.IsNullOrEmpty(type) && !mo.ObjectTypes.Contains(type)) mo.ObjectTypes.Add(type);

            ApplyModComponent(mo, config.antiTechSensor, (AntiTechSensor)null);
            ApplyModComponent(mo, config.bioSensor, (BioSensor)null);
            ApplyModComponent(mo, config.exhibit, (Exhibit)null);
            ApplyModComponent(mo, config.exoCorpse, (ExoCorpse)null);
            ApplyModComponent(mo, config.holoGuide, (HoloGuide)null);
            ApplyModComponent(mo, config.holoPlanet, (HoloPlanet)null);
            ApplyModComponent(mo, config.motionShaft, (MotionShaft)null);
            ApplyModComponent(mo, config.prop, (VanillaProp)null);
            ApplyModComponent(mo, config.quantumEntityState, (QuantumEntityState)null);
            ApplyModComponent(mo, config.scanPulse, (ScanPulse)null);
            ApplyModComponent(mo, config.signalBarrier, (SensorBarrier)null);
            ApplyModComponent(mo, config.signalDoor, (SensorDoor)null);
            ApplyModComponent(mo, config.signalEmitter, (SensorEmitter)null);
            ApplyModComponent(mo, config.signalLight, (SensorLight)null);
        }

        private TComponent ApplyModComponent<TConfig, TComponent>(WrongWarpObject mo, TConfig config, TComponent _) where TConfig : ComponentConfig where TComponent : WrongWarpBehaviour, IConfigurable<TConfig>
        {
            if (config == null) return null;
            var c = mo.gameObject.GetComponent<TComponent>();
            if (!c)
            {
                c = mo.gameObject.AddComponent<TComponent>();
                c.ModObject = mo;
            }
            if (config.disabled.HasValue) c.enabled = !config.disabled.Value;
            c.ApplyConfig(config);
            return c;
        }
    }
}
