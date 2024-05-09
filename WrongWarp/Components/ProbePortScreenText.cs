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
            var block = Text.TextBlocks[BlockIndex];
            output += block.Text + "\n\n";
            output += $"[Page {BlockIndex + 1} / {Text.TextBlocks.Count}]";
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
