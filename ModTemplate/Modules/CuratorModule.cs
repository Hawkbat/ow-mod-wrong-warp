using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Modules
{
    public class CuratorModule : WrongWarpModule
    {
        Queue<NotificationData> notifications = new Queue<NotificationData>();
        NotificationData currentNotification = null;

        public CuratorModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            QueueMessage("Welcome home!", 3f);
        }

        public override void OnSystemUnload()
        {
            notifications.Clear();
        }

        public override void OnUpdate()
        {
            var playerSuit = Locator.GetPlayerSuit();
            if (playerSuit && playerSuit.IsWearingSuit() && notifications.Count > 0 && currentNotification == null)
            {
                currentNotification = notifications.Dequeue();
                NotificationManager.SharedInstance.PostNotification(currentNotification, true);
                DoAfterSeconds(currentNotification.minDuration, () =>
                {
                    NotificationManager.SharedInstance.UnpinNotification(currentNotification);
                    currentNotification = null;
                });
            }
        }

        public void QueueMessage(string msg, float duration)
        {
            notifications.Enqueue(new NotificationData(NotificationTarget.All, msg.ToUpper(), duration, true));
        }
    }
}
