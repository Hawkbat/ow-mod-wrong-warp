using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;
using WrongWarp.Objects;

namespace WrongWarp.Components
{
    public class QuantumEntityState : WrongWarpBehaviour, IConfigurable<QuantumEntityStateConfig>
    {
        public string GroupID;
        public bool IsInitialState;
        public float EntangleRadius;
        public bool NeedsSignalJammer;

        private bool wasObserved;

        public void ApplyConfig(QuantumEntityStateConfig config)
        {
            if (config.group != null) GroupID = config.group;
            if (config.isInitialState.HasValue) IsInitialState = config.isInitialState.Value;
            if (config.needsSignalJammer.HasValue) NeedsSignalJammer = config.needsSignalJammer.Value;
        }

        public override void WireUp()
        {
            Mod.QuantumEntities.AddStateToGroup(this);
        }

        public void OnDestroy()
        {
            Mod.QuantumEntities.RemoveStateFromGroup(this);
        }

        public void Update()
        {
            if (Mod.QuantumEntities.IsCurrentState(this))
            {
                var isObserved = IsObserved();
                if (wasObserved && !isObserved)
                {
                    Mod.QuantumEntities.AttemptShuffle(this);
                }
                wasObserved = isObserved;
            } else
            {
                wasObserved = false;
            }
        }

        public bool IsObserved()
        {
            if (NeedsSignalJammer && !Mod.SaveData.SignalJammerActive) return true;

            // TODO: compare with visibility checks from actual quantum objects

            return IsObservedByPlayer() || IsObservedByProbe();
        }

        public bool IsIlluminated()
        {
            if (!IsEntangledWithPlayer()) return true;
            return IsIlluminatedByPlayer() || IsIlluminatedByShip() || IsIlluminatedByProbe();
        }

        public bool IsEntangled()
        {
            return IsEntangledWithPlayer() || IsEntangledWithShip() || IsEntangledWithProbe();
        }

        public bool IsObservedByPlayer()
        {
            if (Camera.main?.transform == null) return false;
            if (IsEntangledWithPlayer() && !IsIlluminated()) return false;
            var dot = Vector3.Dot(Camera.main.transform.forward, transform.position - Camera.main.transform.position);
            if (dot > 0f) return true;
            return false;
        }

        public bool IsObservedByProbe()
        {
            // TODO: implement observation checks for probe cameras
            if (IsEntangledWithProbe() && !IsIlluminated()) return false;
            return false;
        }

        public bool IsIlluminatedByPlayer()
        {
            var flashlight = Locator.GetFlashlight();
            if (!flashlight) return false;
            return (IsObservedByPlayer() || IsEntangledWithPlayer()) && flashlight.IsFlashlightOn();
        }

        public bool IsIlluminatedByShip()
        {
            var ship = Locator.GetShipTransform();
            if (!ship) return false;
            return IsEntangledWithShip() && ship.GetComponentsInChildren<ShipLight>().Any(l => l.IsEmittingLight());
        }

        public bool IsIlluminatedByProbe()
        {
            var probe = Locator.GetProbe();
            if (!probe || !probe.IsLaunched()) return false;
            return IsEntangledWithProbe();
        }

        public bool IsEntangledWithPlayer()
        {
            var player = Locator.GetPlayerTransform();
            if (!player) return false;
            return Vector3.Distance(transform.position, player.position) < EntangleRadius;
        }

        public bool IsEntangledWithShip()
        {
            var ship = Locator.GetShipTransform();
            if (!ship) return false;
            return Vector3.Distance(transform.position, ship.position) < EntangleRadius;
        }

        public bool IsEntangledWithProbe()
        {
            var probe = Locator.GetProbe();
            if (!probe || !probe.IsLaunched()) return false;
            return Vector3.Distance(transform.position, probe.transform.position) < EntangleRadius;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
