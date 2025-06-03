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
        bool isPlayerInside;

        public override void Awake()
        {
            _prebuilt = true;
            SetQuantumSocketsBetter(_sockets);
            base.Awake();
        }

        public override void Start()
        {
            SetSector(null);
            base.Start();
        }

        void FixedUpdate()
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

        public override bool ChangeQuantumState(bool skipInstantVisibilityCheck)
        {
            if (!IsQuantum()) return false;
            var changed = base.ChangeQuantumState(skipInstantVisibilityCheck);
            if (changed && _occupiedSocket != null)
            {
                transform.parent = _occupiedSocket.transform;
                transform.position = _occupiedSocket.transform.TransformPoint(_localOffset);
            }
            if (changed)
            {
                LogUtils.Warn("Quantum exhibit changed state to " + _occupiedSocket);
            } else
            {
                LogUtils.Warn("Quantum exhibit failed to change state");
            }
            return changed;
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
