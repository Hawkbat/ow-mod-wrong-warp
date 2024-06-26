﻿using ModDataTools.Assets;
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

        public override void PickUpItem(Transform holdTranform)
        {
            base.PickUpItem(holdTranform);
            WrongWarpMod.Instance.SaveData[SaveDataFlag.RespawnDisabled] = true;
        }

        public override string GetDisplayName()
        {
            return WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(NameAsset.FullID);
        }
    }
}
