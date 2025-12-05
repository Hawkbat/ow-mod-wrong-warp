using ModDataTools.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class ProbePortScreenText : ProbePortScreen
    {
        public TranslatorTextAsset Text;
        public int BlockIndex;

        public override string GetText()
        {
            var output = "";
            output += Mod.NewHorizonsApi.GetTranslationForDialogue(Text.FullID + "_" + BlockIndex) + "\n\n";
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
            foreach (var factReveal in Text.RevealFacts)
            {
                if (factReveal.TextBlocks.Contains(Text.TextBlocks[BlockIndex]) && factReveal.Fact)
                {
                    Locator.GetShipLogManager().RevealFact(factReveal.Fact.FullID);
                }
            }
        }
    }
}
