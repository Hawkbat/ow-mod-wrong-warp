using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Components;
using WrongWarp.Objects;

namespace WrongWarp.Modules
{
    public class QuantumEntityModule : WrongWarpModule
    {
        readonly Dictionary<string, QuantumGroup> groups = new Dictionary<string, QuantumGroup>();

        public QuantumEntityModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {

        }

        public override void OnSystemUnload()
        {
            groups.Clear();
        }

        public bool IsCurrentState(QuantumEntityState state)
        {
            if (string.IsNullOrEmpty(state.GroupID)) return false;
            if (groups.TryGetValue(state.GroupID, out QuantumGroup group))
            {
                return group.CurrentState == state;
            }
            return false;
        }

        public void AddStateToGroup(QuantumEntityState state)
        {
            if (string.IsNullOrEmpty(state.GroupID)) return;
            if (!groups.ContainsKey(state.GroupID))
            {
                groups[state.GroupID] = new QuantumGroup();
            }
            if (!groups[state.GroupID].States.Contains(state))
            {
                groups[state.GroupID].States.Add(state);
            }
            if (state.IsInitialState && groups[state.GroupID].CurrentState == null)
            {
                groups[state.GroupID].CurrentState = state;
                state.Activate();
            } else
            {
                state.Deactivate();
            }
        }

        public void RemoveStateFromGroup(QuantumEntityState state)
        {
            if (string.IsNullOrEmpty(state.GroupID)) return;
            if (groups.TryGetValue(state.GroupID, out QuantumGroup group))
            {
                if (group.States.Contains(state))
                {
                    group.States.Remove(state);
                }
                if (group.CurrentState == state)
                {
                    group.CurrentState = null;
                    AttemptShuffle(group);
                }
            }
        }

        public void AttemptShuffle(QuantumEntityState state)
        {
            if (string.IsNullOrEmpty(state.GroupID)) return;
            if (groups.TryGetValue(state.GroupID, out QuantumGroup group))
            {
                AttemptShuffle(group);
            }
        }

        public void AttemptShuffle(QuantumGroup group)
        {
            var validStates = group.States.Where(s => !s.IsObserved() && s != group.CurrentState).ToList();
            if (validStates.Count > 0)
            {
                var newState = validStates[UnityEngine.Random.Range(0, validStates.Count)];
                if (group.CurrentState)
                {
                    if (group.CurrentState.IsEntangled())
                    {
                        var t0 = group.CurrentState.transform;
                        var t1 = newState.transform;

                        if (group.CurrentState.IsEntangledWithPlayer())
                        {
                            var player = Locator.GetPlayerBody();
                            var pos = t0.InverseTransformPoint(player.GetPosition());
                            var rot = Quaternion.Inverse(t0.rotation) * player.GetRotation();
                            player.SetPosition(t1.TransformPoint(pos));
                            player.SetRotation(t1.rotation * rot);
                        }
                        if (group.CurrentState.IsEntangledWithShip())
                        {
                            var ship = Locator.GetShipBody();
                            var pos = t0.InverseTransformPoint(ship.GetPosition());
                            var rot = Quaternion.Inverse(t0.rotation) * ship.GetRotation();
                            ship.SetPosition(t1.TransformPoint(pos));
                            ship.SetRotation(t1.rotation * rot);
                        }
                        if (group.CurrentState.IsEntangledWithProbe())
                        {
                            var probe = Locator.GetProbe().GetAttachedOWRigidbody();
                            var pos = t0.InverseTransformPoint(probe.GetPosition());
                            var rot = Quaternion.Inverse(t0.rotation) * probe.GetRotation();
                            probe.SetPosition(t1.TransformPoint(pos));
                            probe.SetRotation(t1.rotation * rot);
                        }
                    }

                    group.CurrentState.Deactivate();
                    group.CurrentState = null;
                }
                group.CurrentState = newState;
                group.CurrentState.Activate();
            }
        }
    }
}
