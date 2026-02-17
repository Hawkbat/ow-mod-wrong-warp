using ModDataTools.Assets;
using ModDataTools.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class ProbePortScreenText : ProbePortScreen
    {
        public TranslatorTextAsset Text;
        public int BlockIndex;
        List<RevealFactInfo> revealFacts = new List<RevealFactInfo>();

        public override void WireUp()
        {
            base.WireUp();

            if (Text)
            {
                // Hacky workaround since TranslatorTextAsset.RevealFacts does not get serialized properly
                // We round-trip it through the XML file instead
                var modBasePath = WrongWarpMod.Instance.ModHelper.Manifest.ModFolderPath;
                var modRelativePath = Text.GetXmlOutputPath();
                var fullPath = System.IO.Path.Combine(modBasePath, modRelativePath);
                var xmlString = System.IO.File.ReadAllText(fullPath);

                var textXml = new XmlDocument();
                textXml.LoadXml(xmlString);
                var shipLogConditionsNodes = textXml.SelectNodes("NomaiObject/ShipLogConditions");
                if (shipLogConditionsNodes == null)
                {
                    LogUtils.Warn("No ShipLogConditions found in TranslatorTextAsset XML at path: " + modRelativePath);
                    return;
                }
                foreach (var node in shipLogConditionsNodes.OfType<XmlNode>())
                {
                    var revealFactNode = node.SelectSingleNode("RevealFact");
                    // Fact ID to reveal
                    var factID = revealFactNode.SelectSingleNode("FactID").InnerText;
                    // Comma-separated indices of text blocks (starting from 1)
                    var condition = revealFactNode.SelectSingleNode("Condition").InnerText.Split(',').Select(s => int.Parse(s.Trim()));
                    revealFacts.Add(new RevealFactInfo()
                    {
                        FactID = factID,
                        TextBlockIDs = condition.Select(i => i - 1).ToList()
                    });
                }
            }
        }

        public override string GetText()
        {
            var output = "";
            var block = Text.TextBlocks[BlockIndex];
            output += Mod.NewHorizonsApi.GetTranslationForDialogue(block.FullID) + "\n\n";
            var controls = $"[{BlockIndex + 1} / {Text.TextBlocks.Count}]";
            if (BlockIndex > 0)
            {
                controls = "<< " + controls;
            }
            else
            {
                controls = "    " + controls;
            }
            if (BlockIndex < Text.TextBlocks.Count - 1)
            {
                controls += " >>";
            }
            else
            {
                controls += "    ";
            }
            output += controls;
            return output;
        }

        public override bool OnTick(int dx, int dy)
        {
            BlockIndex += dx;
            if (BlockIndex < 0 || BlockIndex >= Text.TextBlocks.Count)
            {
                BlockIndex = Mathf.Clamp(BlockIndex, 0, Text.TextBlocks.Count - 1);
                return false;
            }
            ReadTextBlock();
            return true;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            ReadTextBlock();
        }

        void ReadTextBlock()
        {
            if (Text.RevealFacts != null && Text.RevealFacts.Count > 0)
            {
                // If the serialized RevealFacts worked properly, we just use that
                foreach (var factReveal in Text.RevealFacts)
                {
                    if (factReveal.TextBlocks.Contains(Text.TextBlocks[BlockIndex]) && factReveal.Fact)
                    {
                        Locator.GetShipLogManager().RevealFact(factReveal.Fact.FullID);
                    }
                }
            }
            else if (revealFacts.Count > 0)
            {
                // Use the hacky workaround to reveal facts
                foreach (var factInfo in revealFacts)
                {
                    if (factInfo.TextBlockIDs.Contains(BlockIndex))
                    {
                        Locator.GetShipLogManager().RevealFact(factInfo.FactID);
                    }
                }
            }
        }

        class RevealFactInfo
        {
            public string FactID;
            public List<int> TextBlockIDs;
        }
    }
}
