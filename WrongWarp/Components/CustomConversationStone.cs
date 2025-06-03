using ModDataTools.Assets;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class CustomConversationStone : NomaiConversationStone
    {
        public FrequencyAsset NameAsset;

        NomaiConversationManager _conversationManager;

        public override void Awake()
        {
            base.Awake();

            if (!EnumUtils.IsDefined<NomaiWord>(NameAsset.FullID))
            {
                _word = EnumUtils.Create<NomaiWord>(NameAsset.FullID);
            }
            else
            {
                _word = EnumUtils.Parse<NomaiWord>(NameAsset.FullID);
            }

            _conversationManager = FindObjectOfType<NomaiConversationManager>();

            Reveal();
        }

        protected void Start()
        {
            var socketParent = Locator.GetAstroObject(AstroObject.Name.QuantumMoon).transform.Find("Sector_QuantumMoon/State_EYE/Interactables_EYEState/ConversationPivot/NomaiConversation/ResponseStone/ArcSocket");
            foreach (var wallText in socketParent.GetComponentsInChildren<NomaiWallText>(true))
            {
                if (wallText.gameObject.name.StartsWith("WrongWarp_"))
                {
                    var suffix = wallText.gameObject.name.Substring("WrongWarp_".Length);
                    var nomaiWord = EnumUtils.Parse<NomaiWord>(suffix);
                    ArrayHelpers.Append(ref _conversationManager._questions, new NomaiConversationManager.StonePair()
                    {
                        wordA = _word,
                        wordB = nomaiWord,
                        response = wallText,
                    });
                    wallText.HideImmediate();
                }
            }
        }

        public override string GetDisplayName()
        {
            return WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(NameAsset.FullID);
        }
    }
}
