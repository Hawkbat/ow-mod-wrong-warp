using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class ExoText : Sensor
    {
        public static List<ExoText> All = new();

        public Color ActiveColor;
        public Color InactiveColor;
        public Texture2D TextTexture;
        public ModDataTools.Assets.TranslatorTextBlockAsset TextBlock;

        NomaiWallText text;
        NomaiTextLine textLine;
        NomaiTranslator translator;

        bool isHighlighted = false;

        readonly int mainTexPropID = Shader.PropertyToID("_MainTex");

        public bool IsHighlighted => isHighlighted;
        public NomaiTextLine TextLine => textLine;

        void Awake()
        {
            All.Add(this);
        }

        void OnDestroy()
        {
            All.Remove(this);
        }

        public override void WireUp()
        {
            text = GetComponentInChildren<NomaiWallText>();
            textLine = text._textLines.First(t => t.GetEntryID() == TextBlock.TranslatorText.TextBlocks.IndexOf(TextBlock) + 1);

            translator = Locator.GetToolModeSwapper()._translator;

        }

        public override float ComputeStrength() => isHighlighted && textLine && textLine.IsTranslated() ? 1f : 0f;

        public override void Update()
        {
            base.Update();
            if (translator == null || textLine == null) return;

            if (textLine._renderer.sharedMaterial.mainTexture != TextTexture)
            {
                textLine._renderer.SetMaterialProperty(mainTexPropID, TextTexture);
            }

            if (translator._lastHighlightedTextLine == textLine)
            {
                isHighlighted = true;
            }
            if (WasActivatedThisFrame(this) || WasDeactivatedThisFrame(this))
            {
                textLine.StartColorChangeAnim(textLine._currentColor, IsActivated(this) ? ActiveColor : InactiveColor);
            }
            if (isHighlighted && (!translator.isActiveAndEnabled || translator._currentNomaiText != text || translator._lastHighlightedTextLine != textLine))
            {
                isHighlighted = false;
                var data = text._dictNomaiTextData[textLine._entryID];
                data.IsTranslated = false;
                text._dictNomaiTextData[textLine._entryID] = data;
                textLine.StartColorChangeAnim(textLine.DetermineTextLineColor(textLine._state), textLine.DetermineTextLineColor(NomaiTextLine.VisualState.UNREAD));
                textLine._state = NomaiTextLine.VisualState.UNREAD;
                translator._translatorProp.ClearNomaiText();
                translator._translatorProp.ClearNomaiTextLine();
                translator._lastHighlightedTextLine = null;
                translator._lastLineWasTranslated = false;
                translator._lastLineLocked = false;
            }
        }
    }
}
