using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Components;
using WrongWarp.Configs;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class DeviceSignalModule : WrongWarpModule
    {

        private List<AudioSignal> allSignals = new List<AudioSignal>();
        private List<AudioSignal> availableSignals = new List<AudioSignal>();
        private Dictionary<AudioSignal, AudioSignalDetectionTrigger> signalTriggers = new Dictionary<AudioSignal, AudioSignalDetectionTrigger>();
        private readonly HashSet<SignalName> signalsKnown = new HashSet<SignalName>();
        private readonly Dictionary<AudioSignal, SensorEmitter> allocations = new Dictionary<AudioSignal, SensorEmitter>();
        private bool bypassCustomNamesHack;
        
        public DeviceSignalModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            PopulateSignals();
        }

        public override void OnSystemUnload()
        {
            CleanupSignals();
        }

        private void PopulateSignals()
        {
            foreach (var emitter in allocations.Values) DeallocateSignal(emitter);
            allocations.Clear();

            allSignals = UnityEngine.Object.FindObjectsOfType<AudioSignal>()
                .Where(s => Mod.TweakConfig.signalPool.ContainsKey(GetOriginalName(s.GetName()).ToUpper()))
                .ToList();
            availableSignals = new List<AudioSignal>(allSignals);
            signalTriggers = UnityEngine.Object.FindObjectsOfType<AudioSignalDetectionTrigger>()
                .Where(t => allSignals.Contains(t._signal))
                .ToDictionary(t => t._signal);

            signalsKnown.Clear();

            foreach (var signal in allSignals)
            {
                if (ShouldDeactivateSignal(signal))
                    signal.SetSignalActivation(false);
            }

            if (allSignals.Count == 0)
            {
                LogUtils.Error("Failed to locate any signals!");
            }
        }

        private void CleanupSignals()
        {
            foreach (var emitter in allocations.Values) DeallocateSignal(emitter);
            allocations.Clear();
            allSignals.Clear();
            availableSignals.Clear();
            signalTriggers.Clear();
            signalsKnown.Clear();
        }

        public bool IsManagedSignal(SignalName signalName)
        {
            return allSignals.Any(s => s.GetName() == signalName);
        }

        public bool IsSignalKnown(SignalName signalName)
        {
            return signalsKnown.Contains(signalName);
        }

        public void LearnSignal(SignalName signalName)
        {
            signalsKnown.Add(signalName);
        }

        public void ForgetSignal(SignalName signalName)
        {
            signalsKnown.Remove(signalName);
        }

        public void TryAllocateSignal(SensorEmitter emitter)
        {
            if (emitter.Signal) return;

            if (availableSignals.Count > 0)
            {
                var signal = availableSignals.Find(s => {
                    var name = GetOriginalName(s.GetName()).ToUpper();
                    if (!string.IsNullOrEmpty(emitter.SignalName) && !name.Equals(emitter.SignalName.ToUpper())) return false;
                    if (!string.IsNullOrEmpty(emitter.SignalPrefix) && !name.StartsWith(emitter.SignalPrefix.ToUpper())) return false;
                    if (!string.IsNullOrEmpty(emitter.SignalSuffix) && !name.EndsWith(emitter.SignalSuffix.ToUpper())) return false;
                    return true;
                });
                if (signal)
                {
                    signal.transform.SetParent(emitter.transform, false);
                    if (signalTriggers.TryGetValue(signal, out AudioSignalDetectionTrigger trigger))
                    {
                        trigger.transform.SetParent(emitter.transform, false);
                    }
                    availableSignals.Remove(signal);
                    emitter.Signal = signal;
                    emitter.Signal.SetSignalActivation(true);
                } else
                {
                    var signalNames = string.Join(", ", availableSignals.Select(s => GetOriginalName(s.GetName()).ToUpper()));
                    LogUtils.Log($"Emitter wanted a signal matching '{emitter.SignalName}' or prefix '{emitter.SignalPrefix}' or suffix '{emitter.SignalSuffix}' but no match was found in list: {signalNames}");
                }
            }
        }

        public void DeallocateSignal(SensorEmitter emitter)
        {
            if (emitter.Signal && !availableSignals.Contains(emitter.Signal))
            {
                availableSignals.Add(emitter.Signal);
                if (ShouldForgetSignal(emitter.Signal))
                    ForgetSignal(emitter.Signal.GetName());
                emitter.Signal.SetSignalActivation(false);
                emitter.Signal = null;
            }
        }

        public bool ShouldForgetSignal(AudioSignal signal)
        {
            if (signal == null) return false;
            string name = GetOriginalName(signal.GetName());
            if (Mod.TweakConfig.signalPool.TryGetValue(name.ToUpper(), out SignalConfig config))
            {
                return config.alwaysForget;
            }
            return false;
        }

        public bool ShouldDeactivateSignal(AudioSignal signal)
        {
            if (signal == null) return false;
            string name = GetOriginalName(signal.GetName());
            if (Mod.TweakConfig.signalPool.TryGetValue(name.ToUpper(), out SignalConfig config))
            {
                return config.startDisabled;
            }
            return false;
        }

        public string GetOriginalName(SignalName signalName)
        {
            bypassCustomNamesHack = true;
            var result = AudioSignal.SignalNameToString(signalName);
            bypassCustomNamesHack = false;
            return result;
        }

        public string GetCustomDisplayName(string originalName)
        {
            if (bypassCustomNamesHack) return originalName;
            if (Mod.TweakConfig.signalPool.TryGetValue(originalName.ToUpper(), out SignalConfig config))
            {
                if (!string.IsNullOrEmpty(config.displayName))
                {
                    return config.displayName.ToUpper();
                }
            }
            return originalName;
        }
    }
}
