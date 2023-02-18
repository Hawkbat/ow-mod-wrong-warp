using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Components;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class EyeSequenceModule : WrongWarpModule
    {
        public const string START_PLAYING_CONDITION = "WW_TRAVELER_PLAY";

        bool startedFinale;

        public EyeSequenceModule(WrongWarpMod mod) : base(mod) {
            GlobalMessenger<EyeState>.AddListener("EyeStateChanged", OnEyeStateChanged);
        }

        private void OnEyeStateChanged(EyeState state)
        {
            if (state == EyeState.InstrumentHunt)
            {
                DoAfterFrames(10, () =>
                {
                    SetupEyeSequence();
                });
            }
        }

        /*
         * 
         * Implement changes using QuantumCampsiteController and TravelerEyeController
         * QuantumCampsiteController controls the general sequence,
         *   positioning the various travelers, toggling Solanum and the Prisoner on and off,
         *   and choosing the final music clip with all travelers playing
         * TravelerEyeController drives each individual traveler
         * CosmicInflationController controls the final part of the sequence (end music, expansion effect, etc.)
         * 
         * QuantumCampsiteController stores the list of TravelerEyeControllers
         * We should add the new guy to _travelerControllers (controllers) and _travelerRoots (transform parents)
         * We also need to do this right after:
         * this._travelerControllers[i].OnStartPlaying += this.OnTravelerStartPlaying;
         * 
         * _travelerRoots are placed in the vanilla positions, but if both solanum and prisoner are present,
         * the positions and rotations of _altTravelerSockets are used instead
         * 
         * If we have a puzzle here, we can put it in _instrumentZones
         *   (but we'll need to patch all show/hide methods to actively turn them on)
         * Instrument zone 0 is banjo zone, gets replaced with prisoner zone (5) if prisoner is enabled
         * Instruments have QuantumInstrument and InteractReceiver components and an AudioSignal
         * QuantumInstrument specifies what to activate and deactivate
         *   (the _travelerRoot and the _instrumentZone children, respectively)
         * 
         * We also want to create other campfires in the background
         * We can start spawning those when _waitForMusicEnd becomes true in CosmicInflationController
         * 
         * TravelerEyeController expects:
         * - An Animator with a "Playing" bool and a "PlayingInstrument" state
         * - An AudioSignal for the instrumental track
         * - A CharacterDialogueTree to detect when to start playing (see ConversationVolume)
         * - A dialogue condition to mark when to start playing
         * 
         * CharacterDialogueTree should use the dialogue conditions "AllTravelersGathered" and "AnyTravelersGathered"
         *   to determine whether to show the dialogue option that sets the condition to start playing
         * "JamSessionIsOver" condition for dialogue after stopping playing
         */

        public bool IsApostateEnding()
        {
            return true;
        }

        public void SetupEyeSequence()
        {
            var camp = GameObject.FindObjectOfType<QuantumCampsiteController>();
            var inflation = camp.transform.Find("InflationController").GetComponent<CosmicInflationController>();
            var campsite = camp.transform.Find("Campsite");
            var altTravelerSockets = campsite.Find("AltTravelerSockets");
            
            var pivot = new GameObject("Apostate").transform;
            pivot.parent = campsite.transform;
            pivot.localPosition = Mod.TweakConfig.eyeSequence.travelerPosition;
            pivot.localEulerAngles = Mod.TweakConfig.eyeSequence.travelerRotation;

            var altPivot = new GameObject("Apostate_Alt").transform;
            altPivot.SetParent(altTravelerSockets, false);
            altPivot.localPosition = Mod.TweakConfig.eyeSequence.altTravelerPosition;
            altPivot.localEulerAngles = Mod.TweakConfig.eyeSequence.altTravelerRotation;

            var travelerPrefab = Mod.SystemAssetBundle.LoadAsset<GameObject>("Assets/ModAssets/Shared/Objects/ApostateTraveler.prefab");
            travelerPrefab.SetActive(false);
            var traveler = GameObject.Instantiate(travelerPrefab, pivot, false);

            var signal = Mod.NewHorizonsApi.SpawnSignal(Mod, pivot.gameObject, "audio/signals/TravelerTheme_Apostate.wav", "Apostate", "Traveler");
            signal.transform.localPosition = Vector3.up;
            signal._startActive = false;
            signal._canBePickedUpByScope = false;
            signal._onlyAudibleToScope = false;
            DoAfterFrames(2, () =>
            {
                signal.SetSignalActivation(false, 0f);
                signal._owAudioSource.time = camp._travelerControllers[0]._signal._owAudioSource.time;
            });

            var dialogueTree = Mod.NewHorizonsApi.SpawnDialogue(Mod, pivot.gameObject, "dialogue/EyeOfTheUniverse/Traveler_Apostate.xml", 2f, 2f).Item1;
            dialogueTree.transform.localPosition = Vector3.up;

            var travelerController = pivot.gameObject.AddComponent<TravelerEyeController>();
            travelerController._animator = travelerPrefab.GetComponent<Animator>();
            travelerController._dialogueTree = dialogueTree;
            travelerController._isPlaying = false;
            travelerController._rockingChairAnimator = null;
            travelerController._signal = signal;
            travelerController._startPlayingCondition = START_PLAYING_CONDITION;
            travelerController.OnStartPlaying += camp.OnTravelerStartPlaying;
            travelerController.OnStartPlaying += inflation.OnTravelerStartPlaying;

            traveler.SetActive(true);

            camp._travelerControllers = camp._travelerControllers.Append(travelerController).ToArray();
            camp._travelerRoots = camp._travelerRoots.Append(pivot).ToArray();
            camp._altTravelerSockets = camp._altTravelerSockets.Append(altPivot).ToArray();

            inflation._travelers = inflation._travelers.Append(travelerController).ToArray();
            inflation._inflationObjects = inflation._inflationObjects.Append(pivot).ToArray();

            startedFinale = false;
        }

        public void StartFinale()
        {
            if (startedFinale) return;
            startedFinale = true;
            Mod.StartCoroutine(DoSpawnCampfires());
        }

        public IEnumerator DoSpawnCampfires()
        {
            var camp = GameObject.FindObjectOfType<QuantumCampsiteController>();
            var inflation = camp.transform.Find("InflationController").GetComponent<CosmicInflationController>();
            var t = camp._campfire.transform.parent;
            var planet = t.root.gameObject;
            var sector = camp.GetComponent<Sector>();
            var offset = new Vector3(0f, 0f, 7500f);

            var minRadius = 40f;
            var maxRadius = 150f;

            var campfiresPerBurst = 20;

            var beatsPerSecond = 92f / 60f;
            var secondsPerBeat = 1f / beatsPerSecond * 4f;

            var beatCount = 8f;

            var shadowPuppetPrefabs = new List<GameObject>();
            for (var i = 0; i < 4; i++)
            {
                var prefab = Mod.SystemAssetBundle.LoadAsset<GameObject>($"Assets/ModAssets/Shared/Objects/ShadowPuppets_{i}.prefab");
                prefab.SetActive(false);
                shadowPuppetPrefabs.Add(prefab);
            }

            var campfires = new List<EyeCampfire>();

            for (var i = 0; i < beatCount; i++)
            {
                for (var j = 0; j < campfiresPerBurst; j++)
                {
                    var d = UnityEngine.Random.insideUnitCircle.normalized;
                    var r = Mathf.Lerp(maxRadius, minRadius, Mathf.InverseLerp(0, beatCount - 1, i));
                    r += UnityEngine.Random.Range(-10f, 10f);
                    var o = Mod.NewHorizonsApi.SpawnObject(planet, sector, "EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/QuantumCampfire/Prefab_HEA_Campfire", offset + new Vector3(d.x * r, 0f, d.y * r), Vector3.up * UnityEngine.Random.value * 360f, 1f, false);
                    var campfire = o.AddComponent<EyeCampfire>();
                    var shadowPuppets = GameObject.Instantiate(shadowPuppetPrefabs[UnityEngine.Random.Range(0, shadowPuppetPrefabs.Count)], o.transform, false);
                    shadowPuppets.SetActive(true);
                    campfire.ShadowPuppets = shadowPuppets;
                    campfires.Add(campfire);
                }
                yield return new WaitForSeconds(secondsPerBeat);
            }

            UnityUtils.DoWhen(Mod, () => inflation._state == CosmicInflationController.State.ReadyToCollapse, () =>
            {
                foreach (var campfire in campfires)
                {
                    campfire.HideCampfire(false);
                }
            });
        }

        public AudioClip GetEndingClip(bool hasSolanum, bool hasPrisoner)
        {
            var audioClip = Mod.SystemAssetBundle.LoadAsset<AudioClip>("Assets/ModAssets/Shared/Audio/TravelerTheme_Finale_Apostate.wav");
            audioClip.LoadAudioData();
            return audioClip;
        }
    }
}
