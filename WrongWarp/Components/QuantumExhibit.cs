using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class QuantumExhibit : SocketedQuantumObject
    {
        public DarkZone darkZone;

        bool isPlayerInside;

        public override void Awake()
        {
            _prebuilt = true;
            base.Awake();
            SetSector(null);
            enabled = true;
        }

        protected void FixedUpdate()
        {
            if (!transform.localPosition.ApproxEquals(Vector3.zero))
            {
                LogUtils.Warn("Quantum Exhibit is out of position: " + transform.localPosition + " parent: " + transform.parent + " socket: " + _occupiedSocket);
                transform.parent = _occupiedSocket.transform;
                transform.localPosition = Vector3.zero;
            }

            var dist = Vector3.Distance(transform.position, Locator.GetPlayerCamera().transform.position);
            var wasPlayerInside = isPlayerInside;
            isPlayerInside = dist < _illuminationRadius;
            if (isPlayerInside != wasPlayerInside)
            {
                LogUtils.Warn("Player inside: " + isPlayerInside);
            }
        }

        public override bool IsPlayerEntangled()
        {
            return isPlayerInside;
        }

        public override void OnSectorOccupantsUpdated()
        {
            CheckEnabled();
        }

        public override void OnSocketObscured(QuantumSocket socket)
        {
            LogUtils.Log($"Socket obscured: {socket.name}");
            if (!_socketCollapseDirty && !socket.IsOccupied())
            {
                if (!socket.GetVisibilityObject().IsIlluminated() && socket.GetVisibilityObject().CheckPointInside(Locator.GetPlayerCamera().transform.position))
                {
                    LogUtils.Log("Player is inside the newly obscured socket, skipping collapse");
                    return;
                }
                LogUtils.Log("Collapsing quantum exhibit due to newly obscured socket: " + socket.name);
                _failedNewlyObscuredSocketRolls = 0;
                _recentlyObscuredSocket = socket;
                _socketCollapseDirty = AttemptCollapse();
                _recentlyObscuredSocket = null;
            }
        }

        public override bool CheckIllumination()
        {
            if (!isPlayerInside || !darkZone._playerInDarkZone) return true;
            // Dirty hack to prevent socket illumination from overriding the exhibit's illumination
            var occupiedSocket = _occupiedSocket;
            _occupiedSocket = null;
            var isLit = base.CheckIllumination();
            _occupiedSocket = occupiedSocket;
            return isLit;
        }

        public override bool ChangeQuantumState(bool skipInstantVisibilityCheck)
        {
            if (!IsQuantum()) return false;
            if (isPlayerInside)
            {
                // If the player is inside, only allow state changes if we're intentionally entangle-teleporting
                if (!darkZone._playerInDarkZone) return false;
                skipInstantVisibilityCheck = true;
            }

            var changed = ChangeQuantumStateBetter(skipInstantVisibilityCheck);
            if (changed && _occupiedSocket != null)
            {
                transform.parent = _occupiedSocket.transform;
                transform.position = _occupiedSocket.transform.TransformPoint(_localOffset);

                if (darkZone._ambientLight != null)
                {
                    darkZone._ambientLight.intensity = darkZone._origAmbientIntensity;
                }
                darkZone._ambientLight = _occupiedSocket.transform.root.Find("Sector/AmbientLight").GetComponent<Light>();
                darkZone._origAmbientIntensity = darkZone._ambientLight.intensity;
                if (darkZone._playerInDarkZone)
                {
                    darkZone._ambientLight.intensity = 0f;
                }
            }
            if (changed)
            {
                LogUtils.Warn("Quantum exhibit changed state to " + _occupiedSocket);
            }
            else
            {
                LogUtils.Warn("Quantum exhibit failed to change state");
            }
            return changed;
        }

        bool ChangeQuantumStateBetter(bool skipInstantVisibilityCheck)
        {
            // Vanilla tries random sockets until it finds one that isn't visible, but we can pre-filter instead
            var validSockets = _socketList.Where(s => !s.IsOccupied() && s.IsActive() && (skipInstantVisibilityCheck || !s.GetVisibilityObject().IsVisible())).ToList();
            if (validSockets.Count == 0)
            {
                LogUtils.Warn("No valid sockets to collapse into");
                return false;
            }
            if (_recentlyObscuredSocket != null)
            {
                // Vanilla moves directly to a newly obscured socket, bypassing visibility checks, but we don't
                _recentlyObscuredSocket = null;
            }
            var randomSocket = validSockets[UnityEngine.Random.Range(0, validSockets.Count)];
            MoveToSocket(randomSocket);
            return true;
        }

        public void SetQuantumSocketsBetter(QuantumSocket[] sockets)
        {
            foreach (var socket in _socketList)
            {
                socket.OnNewlyObscured -= OnSocketObscured;
            }
            _sockets = sockets;
            _socketList.Clear();
            foreach (var socket in _sockets)
            {
                _socketList.Add(socket);
            }
            foreach (var socket in _socketList)
            {
                socket.OnNewlyObscured += OnSocketObscured;
            }
        }
    }
}
