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
    public class MuseumModule : WrongWarpModule
    {
        public readonly Material HologramMaterial;

        private readonly List<string> playedNotes = new List<string>();

        public MuseumModule(WrongWarpMod mod) : base(mod)
        {
            HologramMaterial = Mod.SystemAssetBundle.LoadAsset<Material>("Assets/ModAssets/Shared/Materials/Hologram.mat");
        }

        public override void OnSystemLoad()
        {

        }

        public override void OnSystemUnload()
        {

        }

        public void TriggerNote(Exhibit exhibit)
        {
            if (!Mod.SaveData.ExhibitRestored) return;
            
            var note = exhibit.Note;

            if (Mod.TweakConfig.museumNotes[playedNotes.Count] == note)
            {
                LogUtils.Success($"Played right note {note}");
                playedNotes.Add(note);
                if (playedNotes.Count == Mod.TweakConfig.museumNotes.Count)
                {
                    NoteSequenceComplete();
                    playedNotes.Clear();
                }
            }
            else
            {
                LogUtils.Warn($"Played wrong note {note}");
                playedNotes.Clear();
                if (Mod.TweakConfig.museumNotes.Count > 0 && note == Mod.TweakConfig.museumNotes[0])
                {
                    LogUtils.Warn($"Restarting sequence with first note {note} already played");
                    playedNotes.Add(note);
                }
            }
        }

        private void NoteSequenceComplete()
        {
            Mod.SaveData.HasPlayedMuseumMelody = true;
            LogUtils.Success($"Played right note sequence!");
        }
    }
}
