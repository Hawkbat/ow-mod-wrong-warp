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
                var alienShip = Mod.NewHorizonsApi.GetPlanet("Direlict Ship").GetComponent<OWRigidbody>();
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
                var player = Locator.GetPlayerBody();
                var alienShip = Mod.NewHorizonsApi.GetPlanet("The Direlict").GetComponent<OWRigidbody>();

                if (Mod.SaveData.ArchivistSignalActive)
                {
                    var towerA = Mod.NewHorizonsApi.GetPlanet("The Archivist").GetComponent<OWRigidbody>();
                    displayMessage += $"Archivist: {GetDisplayDistance(towerA, player, alienShip)}\n";
                }
                if (Mod.SaveData.GuideSignalActive)
                {
                    var towerB = Mod.NewHorizonsApi.GetPlanet("The Guide").GetComponent<OWRigidbody>();
                    displayMessage += $"Guide: {GetDisplayDistance(towerB, player, alienShip)}\n";
                }
                if (Mod.SaveData.CuratorSignalActive)
                {
                    var towerC = Mod.NewHorizonsApi.GetPlanet("The Curator").GetComponent<OWRigidbody>();
                    displayMessage += $"Curator: {GetDisplayDistance(towerC, player, alienShip)}\n";
                }

                displayMessage = displayMessage.Trim();
            }
            return displayMessage;
        }

        private string GetDisplayDistance(OWRigidbody a, OWRigidbody b, OWRigidbody c)
        {
            var dist = (c.GetPosition() - a.GetPosition()).magnitude - (b.GetPosition() - a.GetPosition()).magnitude;
            if (Math.Abs(dist) > 1000f) return $"{dist / 1000f:N2}km";
            return $"{Mathf.Round(dist)}m";
        }
    }
}
