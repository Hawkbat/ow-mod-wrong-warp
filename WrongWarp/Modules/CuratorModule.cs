using ModDataTools.Assets;
using NewHorizons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Components;

namespace WrongWarp.Modules
{
    public class CuratorModule : WrongWarpModule
    {
        Queue<NotificationData> notifications = new Queue<NotificationData>();
        NotificationData currentNotification = null;
        ScanPulse scanPulse;

        Queue<DialogueAsset> dialogueQueue = new Queue<DialogueAsset>();
        DialogueAsset currentDialogue = null;
        DialogueNodeAsset currentNode = null;

        public CuratorModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            QueueMessage("Welcome home!", 3f);
            var curator = Mod.NewHorizonsApi.GetPlanet("WW_THE_CURATOR");
            DoAfterFrames(1, () =>
            {
                scanPulse = curator.GetComponentInChildren<ScanPulse>(true);
                scanPulse.Target = Mod.NewHorizonsApi.GetPlanet("WW_CORE").transform;
            });
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

        public void TriggerDialogue(DialogueAsset dialogue)
        {
            if (currentDialogue != null || dialogueQueue.Any())
            {
                dialogueQueue.Enqueue(dialogue);
            }
            else
            {
                StartDialogue(dialogue);
            }
        }

        void StartDialogue(DialogueAsset dialogue)
        {
            currentDialogue = dialogue;

            DialogueNodeAsset entryNode;
            for (int i = dialogue.Nodes.Count - 1; i >= 0; i--)
            {
                entryNode = dialogue.Nodes[i];
            }
        }

        void StartNode(DialogueNodeAsset node)
        {
            currentNode = node;
        }

        void QueueMessage(string msg, float duration)
        {
            notifications.Enqueue(new NotificationData(NotificationTarget.All, msg.ToUpper(), duration, true));
        }
    }
}
