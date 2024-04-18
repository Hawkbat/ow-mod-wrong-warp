using ModDataTools.Assets;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Components;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class CuratorModule : WrongWarpModule
    {
        const float DIALOGUE_NOTIFICATION_LENGTH_CONST = 2f;
        const float DIALOGUE_NOTIFICATION_LENGTH_SCALAR = 0.1f;

        Queue<NotificationData> notifications = new Queue<NotificationData>();
        NotificationData currentNotification = null;
        Action onNotificationsFinished;

        Queue<DialogueAsset> dialogueQueue = new Queue<DialogueAsset>();
        DialogueAsset currentDialogue = null;
        DialogueNodeAsset currentNode = null;
        DialogueNodeAsset.Option currentOption = null;

        ScanPulse scanPulse;

        public CuratorModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
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
                    if (!notifications.Any() && onNotificationsFinished != null)
                    {
                        onNotificationsFinished();
                        onNotificationsFinished = null;
                    }
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

            var entryNode = dialogue.DefaultNode;
            foreach (var node in dialogue.Nodes.Where(n => n.EntryConditions.Any()).Reverse())
            {
                if (DialogueUtils.AllConditionsMet(node.EntryConditions))
                {
                    entryNode = node;
                    break;
                }
            }
            StartNode(entryNode);
        }

        void StartNode(DialogueNodeAsset node)
        {
            LogUtils.Log($"Starting node {node.name}");
            currentNode = node;

            if (!node.Pages.Any() || node.Pages.All(p => string.IsNullOrEmpty(p)))
            {
                EndNode();
                return;
            }

            var pageID = 0;
            foreach (var page in node.Pages)
            {
                var text = Mod.NewHorizonsApi.GetTranslationForDialogue($"{node.FullID}_PAGE_{pageID++}");
                var duration = DIALOGUE_NOTIFICATION_LENGTH_CONST + DIALOGUE_NOTIFICATION_LENGTH_SCALAR * text.Length;
                QueueMessage(text, duration);
            }

            onNotificationsFinished = () => EndNode();
        }

        void EndNode()
        {
            var node = currentNode;
            currentNode = null;

            DialogueUtils.RevealFacts(node.RevealFacts);
            DialogueUtils.SetConditions(node.SetConditions, true);

            if (node.Target != null && DialogueUtils.AllFactsRevealed(node.RequiredTargetFacts))
            {
                StartNode(node.Target);
                return;
            }
            var firstOption = node.Options.FirstOrDefault(o =>
                DialogueUtils.AllFactsRevealed(o.RequiredFacts) &&
                DialogueUtils.AllConditionsMet(o.RequiredConditions) &&
                !DialogueUtils.AnyConditionsMet(o.CancelledConditions));
            if (firstOption != null)
            {
                StartOption(node, firstOption);
                return;
            }

            EndDialogue();
        }

        void StartOption(DialogueNodeAsset node, DialogueNodeAsset.Option option)
        {
            LogUtils.Log($"Starting option {node.name}:{option.Text}");
            if (string.IsNullOrEmpty(option.Text))
            {
                EndOption();
                return;
            }
            var optionIndex = node.Options.IndexOf(option);
            var text = Mod.NewHorizonsApi.GetTranslationForDialogue($"{node.FullID}_OPTION_{optionIndex}");
            var duration = DIALOGUE_NOTIFICATION_LENGTH_CONST + DIALOGUE_NOTIFICATION_LENGTH_SCALAR * text.Length;
            QueueMessage(text, duration);

            onNotificationsFinished = () => EndOption();
        }

        void EndOption()
        {
            var option = currentOption;
            currentOption = null;

            DialogueUtils.SetConditions(option.ConditionsToSet, true);
            DialogueUtils.SetConditions(option.ConditionsToCancel, false);

            if (option.Target != null)
            {
                StartNode(option.Target);
                return;
            }

            EndDialogue();
        }

        void EndDialogue()
        {
            currentDialogue = null;

            if (dialogueQueue.Any())
            {
                StartDialogue(dialogueQueue.Dequeue());
            }
        }

        void QueueMessage(string msg, float duration)
        {
            notifications.Enqueue(new NotificationData(NotificationTarget.All, msg.ToUpper(), duration, true));
        }
    }
}
