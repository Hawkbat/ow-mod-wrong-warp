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
        private NotificationData notification;

        public SignalTowerModule(WrongWarpMod mod) : base(mod)
        {

        }

        public override void OnSystemLoad()
        {
            GlobalMessenger.AddListener("ShipEnterCloakField", OnShipEnterCloakField);
        }

        public override void OnSystemUnload()
        {
            GlobalMessenger.RemoveListener("ShipEnterCloakField", OnShipEnterCloakField);
            if (IsNotificationActivated())
                DeactivateNotification();
        }

        public override void OnUpdate()
        {
            if (!IsNotificationActivated() && ShouldActivateNotification())
                ActivateNotification();
            if (IsNotificationActivated() && !ShouldActivateNotification())
                DeactivateNotification();
        }

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

        private bool ShouldActivateNotification() => Mod.SaveData.ArchivistSignalActive || Mod.SaveData.GuideSignalActive || Mod.SaveData.CuratorSignalActive;

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
            if (Mod.SaveData.ArchivistSignalActive || Mod.SaveData.GuideSignalActive || Mod.SaveData.CuratorSignalActive)
            {
                if (Mod.SaveData.ArchivistSignalActive)
                {
                    displayMessage += $"{GetSignalTowerStatusText(SignalTowerType.Archivist)}\n";
                }
                if (Mod.SaveData.GuideSignalActive)
                {
                    displayMessage += $"{GetSignalTowerStatusText(SignalTowerType.Guide)}\n";
                }
                if (Mod.SaveData.CuratorSignalActive)
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
                case SignalTowerType.Archivist: return Mod.SaveData.ArchivistSignalActive;
                case SignalTowerType.Curator: return Mod.SaveData.CuratorSignalActive;
                case SignalTowerType.Guide: return Mod.SaveData.GuideSignalActive;
            }
            return false;
        }

        public void SetSignalTowerActive(SignalTowerType type, bool active)
        {
            switch (type)
            {
                case SignalTowerType.Archivist:
                    Mod.SaveData.ArchivistSignalActive = active;
                    return;
                case SignalTowerType.Curator:
                    Mod.SaveData.CuratorSignalActive = active;
                    return;
                case SignalTowerType.Guide:
                    Mod.SaveData.GuideSignalActive = active;
                    return;
            }
        }

        public string GetSignalTowerStatusText(SignalTowerType type)
        {
            var player = Locator.GetPlayerBody();
            var alienShip = Mod.NewHorizonsApi.GetPlanet("WW_THE_DIRELICT").GetComponent<OWRigidbody>();

            switch (type)
            {
                case SignalTowerType.Archivist:
                    if (Mod.SaveData.ArchivistSignalActive)
                    {
                        var towerA = Mod.NewHorizonsApi.GetPlanet("WW_THE_ARCHIVIST").GetComponent<OWRigidbody>();
                        return $"Archivist: {GetDisplayDistance(towerA, player, alienShip)}";
                    }
                    return "Archivist: INACTIVE";
                case SignalTowerType.Guide:
                    if (Mod.SaveData.GuideSignalActive)
                    {
                        var towerB = Mod.NewHorizonsApi.GetPlanet("WW_THE_GUIDE").GetComponent<OWRigidbody>();
                        return $"Guide: {GetDisplayDistance(towerB, player, alienShip)}";
                    }
                    return "Guide: INACTIVE";
                case SignalTowerType.Curator:
                    if (Mod.SaveData.CuratorSignalActive)
                    {
                        var towerC = Mod.NewHorizonsApi.GetPlanet("WW_THE_CURATOR").GetComponent<OWRigidbody>();
                        return $"Curator: {GetDisplayDistance(towerC, player, alienShip)}";
                    }
                    return "Curator: INACTIVE";
            }
            return string.Empty;
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
