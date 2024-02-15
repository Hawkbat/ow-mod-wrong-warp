using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Components;
using WrongWarp.Objects;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class QuantumExhibitModule : WrongWarpModule
    {
        List<EyeExhibitState> states = new();
        QuantumSocket defaultSocket;
        QuantumExhibit quantumExhibit;

        int tempLock = 0;

        public QuantumExhibitModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {

        }

        public override void OnSystemUnload()
        {
            states.Clear();
            defaultSocket = null;
            quantumExhibit = null;
        }

        public override void OnUpdate()
        {
            if (quantumExhibit == null) return;
            bool isQuantum = quantumExhibit.IsQuantum();
            bool shouldBeQuantum = Mod.SaveData[SaveDataFlag.SignalJammerDisabled];
            if (isQuantum != shouldBeQuantum)
            {
                UpdateQuantumState(shouldBeQuantum);
            }
        }

        public void ApplyTemporaryLock()
        {
            if (tempLock == 0)
            {
                quantumExhibit.SetIsQuantum(false);
            }
            tempLock++;
        }

        public void ReleaseTemporaryLock()
        {
            tempLock--;
            if (tempLock == 0)
            {
                bool shouldBeQuantum = Mod.SaveData[SaveDataFlag.SignalJammerDisabled];
                quantumExhibit.SetIsQuantum(shouldBeQuantum);
            }
        }

        public void RegisterExhibitState(EyeExhibitState state)
        {
            states.Add(state);
            if (state.QuantumExhibit != null)
            {
                defaultSocket = state.QuantumSocket;
                quantumExhibit = state.QuantumExhibit;
            }
            if (states.Count >= 3)
            {
                quantumExhibit.SetQuantumSocketsBetter(states.Select(s => s.QuantumSocket).ToArray());
                quantumExhibit.SetActivation(true);
                UpdateQuantumState(false);
            }
        }

        void UpdateQuantumState(bool shouldBeQuantum)
        {
            if (!shouldBeQuantum) quantumExhibit.MoveToSocket(defaultSocket);
            quantumExhibit.SetIsQuantum(shouldBeQuantum);
        }
    }
}
