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
        [SerializeField] AmoebaGunCanister chargeAmmo = default;
        [SerializeField] AmoebaGunCanister drainAmmo = default;
        [SerializeField] AmoebaGunCanister oxygenAmmo = default;
        [SerializeField] AmoebaGunCanister healAmmo = default;
        [SerializeField] FrequencyAsset nameAsset = default;
        [SerializeField] AmmoType currentAmmoType = default;
        [SerializeField] Transform muzzle = default;

        AmoebaGunMode currentFiringMode;
        bool preventFiringAfterSwitch;

        [SerializeField] FrequencyAsset firePromptAsset = default;
        [SerializeField] FrequencyAsset altFirePromptAsset = default;
        [SerializeField] FrequencyAsset cancelPromptAsset = default;
        [SerializeField] FrequencyAsset chargeAmmoPromptAsset = default;
        [SerializeField] FrequencyAsset drainAmmoPromptAsset = default;
        [SerializeField] FrequencyAsset oxygenAmmoPromptAsset = default;
        [SerializeField] FrequencyAsset healAmmoPromptAsset = default;

        ScreenPrompt firePrompt;
        ScreenPrompt altFirePrompt;
        ScreenPrompt cancelPrompt;
        Dictionary<AmmoType, ScreenPrompt> ammoPrompts = [];

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
            OnActivate();
        }

        public override void DropItem(Vector3 position, Vector3 normal, Transform parent, Sector sector, IItemDropTarget customDropTarget)
        {
            base.DropItem(position, normal, parent, sector, customDropTarget);
            OnDeactivate();
        }

        public override void SocketItem(Transform socketTransform, Sector sector)
        {
            base.SocketItem(socketTransform, sector);
            OnDeactivate();
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
            var fireInput = inToolMode && OWInput.IsPressed(InputLibrary.lockOn, InputMode.Character) || OWInput.IsPressed(InputLibrary.toolActionPrimary, InputMode.Character);
            var altFireInput = inToolMode && OWInput.IsPressed(InputLibrary.toolActionSecondary, InputMode.Character);
            var cancelInput = inToolMode && OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.Character);
            var switchDrainAmmoInput = inToolMode && OWInput.IsNewlyPressed(InputLibrary.toolOptionUp, InputMode.Character);
            var switchChargeAmmoInput = inToolMode && OWInput.IsNewlyPressed(InputLibrary.toolOptionLeft, InputMode.Character);
            var switchOxygenAmmoInput = inToolMode && OWInput.IsNewlyPressed(InputLibrary.toolOptionDown, InputMode.Character);
            var switchHealAmmoInput = inToolMode && OWInput.IsNewlyPressed(InputLibrary.toolOptionRight, InputMode.Character);

            SetPromptVisibility(inToolMode && !OWTime.IsPaused());

            if (preventFiringAfterSwitch && !fireInput && !altFireInput)
            {
                preventFiringAfterSwitch = false;
            }

            var currentCanister = GetCanister(currentAmmoType);
            if (!currentCanister.IsUnlocked())
            {
                // If the current canister was re-locked, switch to the first available canister
                SetCurrentAmmoType(GetUnlockedCanisters().First().AmmoType);
            }
            if (switchDrainAmmoInput && HasUnlockedAmmo(AmmoType.Drain))
            {
                SetCurrentAmmoType(AmmoType.Drain);
            }
            else if (switchChargeAmmoInput && HasUnlockedAmmo(AmmoType.Charge))
            {
                SetCurrentAmmoType(AmmoType.Charge);
            }
            else if (switchOxygenAmmoInput && HasUnlockedAmmo(AmmoType.Oxygen))
            {
                SetCurrentAmmoType(AmmoType.Oxygen);
            }
            else if (switchHealAmmoInput && HasUnlockedAmmo(AmmoType.Heal))
            {
                SetCurrentAmmoType(AmmoType.Heal);
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
                firePrompt = new ScreenPrompt(InputLibrary.lockOn, InputLibrary.toolActionPrimary, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(firePromptAsset.FullID) + "   <CMD>", ScreenPrompt.MultiCommandType.NONE);
                altFirePrompt = new ScreenPrompt(InputLibrary.toolActionSecondary, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(altFirePromptAsset.FullID) + "   <CMD>");
                cancelPrompt = new ScreenPrompt(InputLibrary.cancel, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(cancelPromptAsset.FullID) + "   <CMD>");

                ammoPrompts = new Dictionary<AmmoType, ScreenPrompt>
                {
                    [AmmoType.Charge] = new ScreenPrompt(InputLibrary.toolOptionLeft, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(chargeAmmoPromptAsset.FullID) + "   <CMD>"),
                    [AmmoType.Drain] = new ScreenPrompt(InputLibrary.toolOptionUp, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(drainAmmoPromptAsset.FullID) + "   <CMD>"),
                    [AmmoType.Oxygen] = new ScreenPrompt(InputLibrary.toolOptionDown, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(oxygenAmmoPromptAsset.FullID) + "   <CMD>"),
                    [AmmoType.Heal] = new ScreenPrompt(InputLibrary.toolOptionRight, WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(healAmmoPromptAsset.FullID) + "   <CMD>"),
                };

                Locator.GetPromptManager().AddScreenPrompt(firePrompt, PromptPosition.UpperRight, false);
                Locator.GetPromptManager().AddScreenPrompt(altFirePrompt, PromptPosition.UpperRight, false);
                Locator.GetPromptManager().AddScreenPrompt(cancelPrompt, PromptPosition.UpperRight, false);
                foreach (var prompt in ammoPrompts.Values)
                {
                    Locator.GetPromptManager().AddScreenPrompt(prompt, PromptPosition.UpperRight, false);
                }

            }

            firePrompt.SetVisibility(visible);
            altFirePrompt.SetVisibility(visible);
            cancelPrompt.SetVisibility(visible && PlayerState.IsWearingSuit());
            foreach (var kv in ammoPrompts)
            {
                kv.Value.SetVisibility(visible && GetCanister(kv.Key).IsUnlocked());
            }
        }

        void OnActivate()
        {
            enabled = true;
            SetPromptVisibility(true);
            transform.localPosition = new Vector3(0.2f, -0.1f, 0.1f);
            GetCanister(currentAmmoType).OnCanisterActivated();
        }

        void OnDeactivate()
        {
            enabled = false;
            SetPromptVisibility(false);
            GetCanister(currentAmmoType).OnCanisterDeactivated();
        }

        void SetCurrentAmmoType(AmmoType ammoType)
        {
            GetCanister(currentAmmoType).OnCanisterDeactivated();
            currentAmmoType = ammoType;
            GetCanister(currentAmmoType).OnCanisterActivated();
            preventFiringAfterSwitch = true;
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
