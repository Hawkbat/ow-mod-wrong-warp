using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Components;

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
                Mod.ModHelper.Console.WriteLine($"Played right note {note}", OWML.Common.MessageType.Success);
                playedNotes.Add(note);
                if (playedNotes.Count == Mod.TweakConfig.museumNotes.Count)
                {
                    NoteSequenceComplete();
                    playedNotes.Clear();
                }
            }
            else
            {
                Mod.ModHelper.Console.WriteLine($"Played wrong note {note}", OWML.Common.MessageType.Warning);
                playedNotes.Clear();
                if (Mod.TweakConfig.museumNotes.Count > 0 && note == Mod.TweakConfig.museumNotes[0])
                {
                    Mod.ModHelper.Console.WriteLine($"Restarting sequence with first note {note} already played", OWML.Common.MessageType.Warning);
                    playedNotes.Add(note);
                }
            }
        }

        private void NoteSequenceComplete()
        {
            Mod.SaveData.HasPlayedMuseumMelody = true;
            Mod.ModHelper.Console.WriteLine($"Played right note sequence!", OWML.Common.MessageType.Success);
        }
    }
}
