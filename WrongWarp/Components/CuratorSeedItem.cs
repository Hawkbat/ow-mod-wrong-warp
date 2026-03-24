using ModDataTools.Assets;
using OWML.Utils;
using System;
using System.Collections;
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

        [SerializeField] MeshRenderer renderer;

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

        public override bool CheckIsDroppable() => false;

        public override string GetDisplayName() =>
            WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(NameAsset.FullID);

        protected void Update()
        {
            var isHeld = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem() == this;
            if (isHeld)
            {
                var heldInShip = PlayerState.AtFlightConsole();
                transform.localPosition = heldInShip ? new Vector3(0.2f, 0.5f, 0.25f) : new Vector3(0f, 0f, 0f);
            }
        }

        public void Flash()
        {
            StartCoroutine(DoFlash());
        }

        IEnumerator DoFlash()
        {
            var initialMat = renderer.sharedMaterial;
            var flashMat = new Material(initialMat);
            renderer.sharedMaterial = flashMat;
            flashMat.SetColor("_EmissionColor", Color.black);
            var t = 0f;
            while (t < 1f)
            {
                flashMat.SetColor("_EmissionColor", Color.Lerp(Color.black, flashMat.color, t));
                t += Time.deltaTime * 0.25f;
                yield return new WaitForEndOfFrame();
            }
            flashMat.SetColor("_EmissionColor", flashMat.color);
        }
    }
}
