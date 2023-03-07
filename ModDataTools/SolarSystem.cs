using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWUnityUtils
{
    [CreateAssetMenu]
    public class SolarSystem : ScriptableObject
    {
        public string Name { get => name; set => name = value; }
        [Tooltip("The file path of the solar system's New Horizons config file")]
        public string FileName;
        [Tooltip("A prefix appended to all entry and fact IDs belonging to planets in this solar system")]
        public string ChildIDPrefix;
        [Tooltip("An override value for the far clip plane. Allows you to see farther.")]
        public bool FarClipPlaneOverrideEnabled;
        [Tooltip("An override value for the far clip plane. Allows you to see farther.")]
        [ConditionalField(nameof(FarClipPlaneOverrideEnabled))]
        public float FarClipPlaneOverride;
        [Tooltip("Whether this system can be warped to via the warp drive")]
        public bool CanEnterViaWarpDrive = true;
        [Tooltip("Do you want a clean slate for this star system? Or will it be a modified version of the original.")]
        public bool DestroyStockPlanets = true;
        [Tooltip("Should the time loop be enabled in this system?")]
        public bool EnableTimeLoop = true;
        [Tooltip("Set to the Fact that must be revealed before it can be warped to.")]
        public ShipLogFactBase FactRequiredForWarp;
        [Tooltip("The duration of the time loop in minutes. This is the time the sun explodes. End Times plays 85 seconds before this time, and your memories get sent back about 40 seconds after this time.")]
        public float LoopDuration = 22f;
        [Tooltip("Should the player not be able to view the map in this system?")]
        public bool MapRestricted;
        [Tooltip("The skybox to show in this system")]
        public SkyboxConfig Skybox;
        [Tooltip("Set to true if you want to spawn here after dying, not Timber Hearth. You can still warp back to the main star system.")]
        public bool StartHere;
        [Tooltip("Set to true if you want the player to stay in this star system if they die in it.")]
        public bool RespawnHere;
        [Tooltip("The music to play while flying between planets")]
        public AudioClip TravelAudio;
        [Tooltip("Whether you can warp to this system with the vessel")]
        public bool HasWarpCoordinates;
        [Tooltip("Settings for the vessel")]
        [ConditionalField(nameof(HasWarpCoordinates))]
        public VesselConfig Vessel;
        [Tooltip("The New Horizons solar system config .json file")]
        public TextAsset ConfigFile;

        [System.Serializable]
        public class SkyboxConfig
        {
            [Tooltip("Whether to destroy the star field around the player")]
            public bool DestroyStarField;
            [Tooltip("Whether to use a cube for the skybox instead of a smooth sphere")]
            public bool UseCube;
            [Tooltip("Texture to use for the skybox's positive X direction")]
            public Texture2D Right;
            [Tooltip("Texture to use for the skybox's negative X direction")]
            public Texture2D Left;
            [Tooltip("Texture to use for the skybox's positive Y direction")]
            public Texture2D Top;
            [Tooltip("Texture to use for the skybox's negative Y direction")]
            public Texture2D Bottom;
            [Tooltip("Texture to use for the skybox's positive Z direction")]
            public Texture2D Front;
            [Tooltip("Texture to use for the skybox's negative Z direction")]
            public Texture2D Back;
        }

        [System.Serializable]
        public class VesselConfig
        {
            [Tooltip("The position in the solar system the vessel will warp to.")]
            public Vector3 VesselPosition;
            [Tooltip("Euler angles by which the vessel will be oriented.")]
            public Vector3 VesselRotation;
            [Tooltip("The relative position to the vessel that you will be teleported to when you exit the vessel through the black hole.")]
            public Vector3 WarpExitPosition;
            [Tooltip("Euler angles by which the warp exit will be oriented.")]
            public Vector3 WarpExitRotation;
            [Tooltip("The warp coordinates to use with the vessel")]
            public Coordinates WarpCoordinates;
        }

        [System.Serializable]
        public class Coordinates
        {
            public int[] x;
            public int[] y;
            public int[] z;
        }
    }
}
