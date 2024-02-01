using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Playables;
using WrongWarp.Components;

namespace WrongWarp.Modules
{
    public class HeatModule : WrongWarpModule
    {
        NotificationData notification;
        OverheatHazardVolume overheatVolume;

        public HeatModule(WrongWarpMod mod) : base(mod)
        {
        }

        public override void OnSystemLoad()
        {
            DoAfterFrames(2, () =>
            {
                overheatVolume = UnityEngine.Object.FindObjectOfType<OverheatHazardVolume>();
            });
        }

        public override void OnSystemUnload()
        {
            overheatVolume = null;
        }

        public override void OnUpdate()
        {
            if (!IsNotificationActivated() && ShouldActivateNotification())
                ActivateNotification();
            if (IsNotificationActivated() && !ShouldActivateNotification())
                DeactivateNotification();
        }

        public OverheatHazardVolume GetOverheatVolume()
        {
            return overheatVolume;
        }

        private bool IsNotificationActivated() => notification != null;

        private bool ShouldActivateNotification() => overheatVolume;

        private void ActivateNotification()
        {
            notification = new NotificationData(NotificationTarget.All, "", 0f, false);
            NotificationManager.SharedInstance.PostNotification(notification, true);
        }

        private void DeactivateNotification()
        {
            NotificationManager.SharedInstance.UnpinNotification(notification);
            notification = null;
        }

        public bool IsActiveNotification(NotificationData data) => notification == data;

        public string GetNotificationText()
        {
            var displayMessage = "";
            if (overheatVolume)
            {
                var pos = Locator.GetPlayerTransform().position;
                displayMessage += $"Temperature: {overheatVolume.GetTemperatureAt(pos):n2} / {overheatVolume.GetCurrentMaxTemperature():n2}\n";
                displayMessage += $"Damage: {overheatVolume.GetRawDamageAt(pos):n2} ({overheatVolume.GetEffectiveDamageToPlayer():n2})\n";
            }
            displayMessage = displayMessage.Trim();
            return displayMessage;
        }
    }
}
