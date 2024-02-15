using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class SignalTowerModule : WrongWarpModule
    {
        NotificationData notification;
        AudioSignal direlictSignal;

        OWRigidbody alienShip;
        OWRigidbody towerA;
        OWRigidbody towerB;
        OWRigidbody towerC;

        public SignalTowerModule(WrongWarpMod mod) : base(mod)
        {

        }

        public override void OnSystemLoad()
        {
            GlobalMessenger.AddListener("ShipEnterCloakField", OnShipEnterCloakField);
            alienShip = Mod.NewHorizonsApi.GetPlanet("WW_THE_DIRELICT").GetComponent<OWRigidbody>();
            towerA = Mod.NewHorizonsApi.GetPlanet("WW_THE_ARCHIVIST").GetComponent<OWRigidbody>();
            towerB = Mod.NewHorizonsApi.GetPlanet("WW_THE_GUIDE").GetComponent<OWRigidbody>();
            towerC = Mod.NewHorizonsApi.GetPlanet("WW_THE_CURATOR").GetComponent<OWRigidbody>();
            direlictSignal = alienShip.GetComponentsInChildren<AudioSignal>().First(s => s.name == "DirelictSignal");
        }

        public override void OnSystemUnload()
        {
            GlobalMessenger.RemoveListener("ShipEnterCloakField", OnShipEnterCloakField);
            if (IsNotificationActivated())
                DeactivateNotification();
            direlictSignal = null;
        }

        public override void OnUpdate()
        {
            if (!IsNotificationActivated() && ShouldActivateNotification())
                ActivateNotification();
            if (IsNotificationActivated() && !ShouldActivateNotification())
                DeactivateNotification();
        }

        public AudioSignal GetDirelictSignal() => direlictSignal;

        private void OnShipEnterCloakField()
        {
            if (Mod.IsInWrongWarpSystem)
            {
                var alienShip = Mod.NewHorizonsApi.GetPlanet("WW_THE_DIRELICT").GetComponent<OWRigidbody>();
                var ship = Locator.GetShipBody();
                ship.SetVelocity(alienShip.GetVelocity() + (alienShip.GetPosition() - ship.GetPosition()).normalized);
            }
        }

        private bool IsNotificationActivated() => notification != null;

        private bool ShouldActivateNotification() => Mod.SaveData[SaveDataFlag.ArchivistSignalActive] || Mod.SaveData[SaveDataFlag.GuideSignalActive] || Mod.SaveData[SaveDataFlag.CuratorSignalActive];

        private void ActivateNotification()
        {
            notification = new NotificationData(NotificationTarget.Ship, "", 0f, false);
            NotificationManager.SharedInstance.PostNotification(notification, true);
        }

        private void DeactivateNotification()
        {
            NotificationManager.SharedInstance.UnpinNotification(notification);
            notification = null;
        }

        public bool IsActiveNotification(NotificationData data)
        {
            return notification == data;
        }

        public string GetNotificationText()
        {
            string displayMessage = "";
            if (Mod.SaveData[SaveDataFlag.ArchivistSignalActive] || Mod.SaveData[SaveDataFlag.GuideSignalActive] || Mod.SaveData[SaveDataFlag.CuratorSignalActive])
            {
                if (Mod.SaveData[SaveDataFlag.ArchivistSignalActive])
                {
                    displayMessage += $"{GetSignalTowerStatusText(SignalTowerType.Archivist)}\n";
                }
                if (Mod.SaveData[SaveDataFlag.GuideSignalActive])
                {
                    displayMessage += $"{GetSignalTowerStatusText(SignalTowerType.Guide)}\n";
                }
                if (Mod.SaveData[SaveDataFlag.CuratorSignalActive])
                {
                    displayMessage += $"{GetSignalTowerStatusText(SignalTowerType.Curator)}\n";
                }

                displayMessage = displayMessage.Trim();
            }
            return displayMessage;
        }

        public string GetProbePortText()
        {
            string displayMessage = "SIGNAL TRACKING\n";

            displayMessage += $"{GetSignalTowerStatusText(SignalTowerType.Archivist)}\n";
            displayMessage += $"{GetSignalTowerStatusText(SignalTowerType.Guide)}\n";
            displayMessage += $"{GetSignalTowerStatusText(SignalTowerType.Curator)}\n";

            displayMessage = displayMessage.Trim();
            return displayMessage;
        }

        public bool IsSignalTowerActive(SignalTowerType type)
        {
            switch (type)
            {
                case SignalTowerType.Archivist: return Mod.SaveData[SaveDataFlag.ArchivistSignalActive];
                case SignalTowerType.Curator: return Mod.SaveData[SaveDataFlag.CuratorSignalActive];
                case SignalTowerType.Guide: return Mod.SaveData[SaveDataFlag.GuideSignalActive];
            }
            return false;
        }

        public void SetSignalTowerActive(SignalTowerType type, bool active)
        {
            switch (type)
            {
                case SignalTowerType.Archivist:
                    Mod.SaveData[SaveDataFlag.ArchivistSignalActive] = active;
                    return;
                case SignalTowerType.Curator:
                    Mod.SaveData[SaveDataFlag.CuratorSignalActive] = active;
                    return;
                case SignalTowerType.Guide:
                    Mod.SaveData[SaveDataFlag.GuideSignalActive] = active;
                    return;
            }
        }

        public OWRigidbody GetSignalTowerBody(SignalTowerType type)
        {
            switch (type)
            {
                case SignalTowerType.Archivist:
                    return towerA;
                case SignalTowerType.Guide:
                    return towerB;
                case SignalTowerType.Curator:
                    return towerC;
            }
            return null;
        }

        public string GetSignalTowerStatusText(SignalTowerType type)
        {
            return $"{type}: {(IsSignalTowerActive(type) ? GetDisplayDistance(GetSignalTowerBody(type), Locator.GetPlayerBody(), alienShip) : "INACTIVE")}";
        }

        public float GetSignalTowerDistance(SignalTowerType type)
        {
            return Vector3.Distance(GetSignalTowerBody(type).GetPosition(), alienShip.GetPosition());
        }

        private string GetDisplayDistance(OWRigidbody a, OWRigidbody b, OWRigidbody c)
        {
            var dist = (c.GetPosition() - a.GetPosition()).magnitude - (b.GetPosition() - a.GetPosition()).magnitude;
            if (Math.Abs(dist) > 1000f) return $"{dist / 1000f:N2}km";
            return $"{Mathf.Round(dist)}m";
        }

        public enum SignalTowerType
        {
            Archivist,
            Guide,
            Curator,
        }
    }
}
