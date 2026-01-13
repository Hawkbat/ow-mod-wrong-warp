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
    public class AmoebaGunItem : OWItem
    {
        [SerializeField] AmoebaGunCanister chargeAmmo;
        [SerializeField] AmoebaGunCanister drainAmmo;
        [SerializeField] AmoebaGunCanister oxygenAmmo;
        [SerializeField] AmoebaGunCanister healAmmo;
        [SerializeField] FrequencyAsset nameAsset;
        [SerializeField] AmmoType currentAmmoType;
        [SerializeField] Transform muzzle;
        AmoebaGunMode currentFiringMode;
        bool preventFiringAfterSwitch;

        [SerializeField] FrequencyAsset firePromptAsset;
        [SerializeField] FrequencyAsset altFirePromptAsset;
        [SerializeField] FrequencyAsset switchAmmoPromptAsset;
        [SerializeField] FrequencyAsset cancelPromptAsset;

        ScreenPrompt firePrompt;
        ScreenPrompt altFirePrompt;
        ScreenPrompt switchAmmoPrompt;
        ScreenPrompt cancelPrompt;

        public static readonly ItemType ItemType;

        static AmoebaGunItem()
        {
            ItemType = EnumUtils.Create<ItemType>(nameof(AmoebaGunItem));
        }

        public override void Awake()
        {
            base.Awake();
            _type = ItemType;
            enabled = false;

        }

        public override string GetDisplayName()
        {
            return WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(nameAsset.FullID);
        }

        public override void PickUpItem(Transform holdTranform)
        {
            base.PickUpItem(holdTranform);
            enabled = true;
            SetPromptVisibility(true);
        }

        public override void DropItem(Vector3 position, Vector3 normal, Transform parent, Sector sector, IItemDropTarget customDropTarget)
        {
            base.DropItem(position, normal, parent, sector, customDropTarget);
            enabled = false;
            SetPromptVisibility(false);
        }

        public override void SocketItem(Transform socketTransform, Sector sector)
        {
            base.SocketItem(socketTransform, sector);
            enabled = false;
            SetPromptVisibility(false);
        }

        public bool HasUnlockedAmmo(AmmoType type) => GetCanister(type).IsUnlocked();

        public float GetAmmoAmount(AmmoType type) => GetCanister(type).GetAmount();

        public bool LockAmmo(AmmoType type) => GetCanister(type).Lock();

        public bool UnlockAmmo(AmmoType type) => GetCanister(type).Unlock();

        public float RestoreAmmo(AmmoType type, float amount) => GetCanister(type).RestoreAmount(amount);

        public float ConsumeAmmo(AmmoType type, float amount) => GetCanister(type).ConsumeAmount(amount);

        public Transform GetMuzzleTransform() => muzzle;

        protected void Update()
        {
            var anyCanistersUnlocked = GetUnlockedCanisters().Any();
            if (!anyCanistersUnlocked)
            {
                // If all canisters have been locked, clean up and stop updating
                if (currentFiringMode)
                {
                    currentFiringMode.SetFiring(false);
                    currentFiringMode = null;
                }

                return;
            }

            var inToolMode = Locator.GetToolModeSwapper().IsInToolMode(ToolMode.Item) && Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem() == this;
            var fireInput = inToolMode && OWInput.IsPressed(InputLibrary.toolActionPrimary, InputMode.Character);
            var altFireInput = inToolMode && OWInput.IsPressed(InputLibrary.toolActionSecondary, InputMode.Character);
            var switchAmmoBackInput = inToolMode && (OWInput.IsNewlyPressed(InputLibrary.toolOptionUp, InputMode.Character) || OWInput.IsNewlyPressed(InputLibrary.toolOptionLeft, InputMode.Character));
            var switchAmmoForwardInput = inToolMode && (OWInput.IsNewlyPressed(InputLibrary.toolOptionDown, InputMode.Character) || OWInput.IsNewlyPressed(InputLibrary.toolOptionRight, InputMode.Character));
            var cancelInput = inToolMode && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Character);

            SetPromptVisibility(inToolMode);

            if (preventFiringAfterSwitch && !fireInput && !altFireInput)
            {
                preventFiringAfterSwitch = false;
            }

            var currentCanister = GetCanister(currentAmmoType);
            if (!currentCanister.IsUnlocked())
            {
                // If the current canister was re-locked, switch to the first available canister
                currentAmmoType = GetUnlockedCanisters().First().AmmoType;
                preventFiringAfterSwitch = true;
            }
            else
            {
                if (switchAmmoBackInput)
                {
                    var unlockedCanisters = GetUnlockedCanisters().ToList();
                    var currentIndex = unlockedCanisters.FindIndex(c => c.AmmoType == currentAmmoType);
                    var newIndex = (currentIndex - 1 + unlockedCanisters.Count) % unlockedCanisters.Count;
                    currentAmmoType = unlockedCanisters[newIndex].AmmoType;
                    preventFiringAfterSwitch = true;
                }
                if (switchAmmoForwardInput)
                {
                    var unlockedCanisters = GetUnlockedCanisters().ToList();
                    var currentIndex = unlockedCanisters.FindIndex(c => c.AmmoType == currentAmmoType);
                    var newIndex = (currentIndex + 1) % unlockedCanisters.Count;
                    currentAmmoType = unlockedCanisters[newIndex].AmmoType;
                    preventFiringAfterSwitch = true;
                }
            }

            // In Drain mode, secondary uses Charge if available
            if (currentAmmoType == AmmoType.Drain && altFireInput && HasUnlockedAmmo(AmmoType.Charge))
            {
                currentCanister = GetCanister(AmmoType.Charge);
            }
            // In all other modes, alt fire uses Drain if available
            else if (altFireInput && HasUnlockedAmmo(AmmoType.Drain))
            {
                currentCanister = GetCanister(AmmoType.Drain);
            }

            var hasAmmo = currentCanister.GetAmount() > 0f;
            var shouldFire = (fireInput || altFireInput) && hasAmmo && !preventFiringAfterSwitch;

            if (currentFiringMode != currentCanister.GetFiringMode())
            {
                if (currentFiringMode)
                {
                    currentFiringMode.SetFiring(false);
                }
                currentFiringMode = currentCanister.GetFiringMode();
            }

            currentFiringMode.SetFiring(shouldFire);

            if (cancelInput && PlayerState.IsWearingSuit())
            {
                InputLibrary.cancel.ConsumeInput();
                Locator.GetToolModeSwapper().EquipToolMode(ToolMode.Probe);
            }
        }

        void SetPromptVisibility(bool visible)
        {
            if (firePrompt == null)
            {
                firePrompt = new ScreenPrompt(InputLibrary.toolActionPrimary, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(firePromptAsset.FullID) + "   <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt));
                altFirePrompt = new ScreenPrompt(InputLibrary.toolActionSecondary, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(altFirePromptAsset.FullID) + "   <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt));
                switchAmmoPrompt = new ScreenPrompt(new List<IInputCommands> { InputLibrary.toolOptionUp, InputLibrary.toolOptionDown, InputLibrary.toolOptionLeft, InputLibrary.toolOptionRight }, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(switchAmmoPromptAsset.FullID) + "   <CMD>" + UITextLibrary.GetString(UITextType.PressPrompt), ScreenPrompt.MultiCommandType.NONE);
                cancelPrompt = new ScreenPrompt(InputLibrary.cancel, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(cancelPromptAsset.FullID) + "   <CMD>" + UITextLibrary.GetString(UITextType.PressPrompt));

                Locator.GetPromptManager().AddScreenPrompt(firePrompt, PromptPosition.UpperRight, false);
                Locator.GetPromptManager().AddScreenPrompt(altFirePrompt, PromptPosition.UpperRight, false);
                Locator.GetPromptManager().AddScreenPrompt(switchAmmoPrompt, PromptPosition.UpperRight, false);
                Locator.GetPromptManager().AddScreenPrompt(cancelPrompt, PromptPosition.UpperRight, false);

            }

            firePrompt.SetVisibility(visible);
            altFirePrompt.SetVisibility(visible);
            switchAmmoPrompt.SetVisibility(visible);
            cancelPrompt.SetVisibility(visible && PlayerState.IsWearingSuit());
        }

        AmoebaGunCanister GetCanister(AmmoType type) => type switch
        {
            AmmoType.Charge => chargeAmmo,
            AmmoType.Drain => drainAmmo,
            AmmoType.Oxygen => oxygenAmmo,
            AmmoType.Heal => healAmmo,
            _ => null,
        };

        IEnumerable<AmoebaGunCanister> GetUnlockedCanisters()
        {
            if (chargeAmmo.IsUnlocked()) yield return chargeAmmo;
            if (drainAmmo.IsUnlocked()) yield return drainAmmo;
            if (oxygenAmmo.IsUnlocked()) yield return oxygenAmmo;
            if (healAmmo.IsUnlocked()) yield return healAmmo;
        }

        public enum AmmoType
        {
            Charge,
            Drain,
            Oxygen,
            Heal,
        }
    }
}
