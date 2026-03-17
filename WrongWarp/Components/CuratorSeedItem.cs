using ModDataTools.Assets;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class CuratorSeedItem : OWItem
    {
        public FrequencyAsset NameAsset;

        public static ItemType ItemType => customItemType;

        static readonly ItemType customItemType;

        static CuratorSeedItem()
        {
            customItemType = EnumUtils.Create<ItemType>(nameof(CuratorSeedItem));
        }

        public override void Awake()
        {
            base.Awake();
            _type = customItemType;
        }

        public override string GetDisplayName() =>
            WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(NameAsset.FullID);
    }
}
