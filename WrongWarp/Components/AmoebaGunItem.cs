using ModDataTools.Assets;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

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
        bool held;
        AmoebaGunMode currentFiringMode;
        bool preventFiringAfterSwitch;

        static readonly ItemType customItemType;

        static AmoebaGunItem()
        {
            customItemType = EnumUtils.Create<ItemType>(nameof(AmoebaGunItem));
        }

        public override void Awake()
        {
            base.Awake();
            _type = customItemType;
            enabled = false;
        }

        public override string GetDisplayName()
        {
            return WrongWarpMod.Instance.NewHorizonsApi.GetTranslationForUI(nameAsset.FullID);
        }

        public override void PickUpItem(Transform holdTranform)
        {
            base.PickUpItem(holdTranform);
            SetHeld(true);
        }

        public override void DropItem(Vector3 position, Vector3 normal, Transform parent, Sector sector, IItemDropTarget customDropTarget)
        {
            base.DropItem(position, normal, parent, sector, customDropTarget);
            SetHeld(false);
        }

        public override void SocketItem(Transform socketTransform, Sector sector)
        {
            base.SocketItem(socketTransform, sector);
            SetHeld(false);
        }

        public bool HasUnlockedAmmo(AmmoType type) => GetCanister(type).IsUnlocked();

        public float GetAmmoAmount(AmmoType type) => GetCanister(type).GetAmount();

        public bool LockAmmo(AmmoType type) => GetCanister(type).Lock();

        public bool UnlockAmmo(AmmoType type) => GetCanister(type).Unlock();

        public float RestoreAmmo(AmmoType type, float amount) => GetCanister(type).RestoreAmount(amount);

        public float ConsumeAmmo(AmmoType type, float amount) => GetCanister(type).ConsumeAmount(amount);

        public Transform GetMuzzleTransform() => muzzle;

        void SetHeld(bool held)
        {
            this.held = held;
            enabled = held;
        }

        protected void Update()
        {
            if (!held) return;

            var anyCanistersUnlocked = GetUnlockedCanisters().Any();
            if (!anyCanistersUnlocked)
            {
                if (currentFiringMode)
                {
                    currentFiringMode.SetFiring(false);
                    currentFiringMode = null;
                }

                return;
            }

            var inToolMode = Locator.GetToolModeSwapper().IsInToolMode(ToolMode.Item);
            var fireInput = inToolMode && OWInput.IsPressed(InputLibrary.toolActionPrimary, InputMode.Character);
            var altFireInput = inToolMode && OWInput.IsPressed(InputLibrary.toolActionSecondary, InputMode.Character);
            var switchAmmoBackInput = inToolMode && (OWInput.IsNewlyPressed(InputLibrary.toolOptionUp, InputMode.Character) || OWInput.IsNewlyPressed(InputLibrary.toolOptionLeft, InputMode.Character));
            var switchAmmoForwardInput = inToolMode && (OWInput.IsNewlyPressed(InputLibrary.toolOptionDown, InputMode.Character) || OWInput.IsNewlyPressed(InputLibrary.toolOptionRight, InputMode.Character));

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
