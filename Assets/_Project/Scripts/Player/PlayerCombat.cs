using System;
using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Core.Actor;
using _Project.Scripts.Core.Input;
using _Project.Scripts.Weapon;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public sealed class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private Transform mainWeaponHolder;
        [SerializeField] private Transform leftWeaponHolder;
        [SerializeField] private Transform rightWeaponHolder;
        
        private WeaponView[] weaponViews = new WeaponView[2];
        
        private Weapon.Weapon[] weaponSlots = new Weapon.Weapon[2];
        private int activeSlotIndex = 0;
        public Weapon.Weapon ActiveWeapon => weaponSlots[activeSlotIndex];
        
        public event Action<Weapon.Weapon> OnWeaponAdded;
        public event Action<Weapon.Weapon> OnWeaponRemoved;
        public event Action<Weapon.Weapon> OnWeaponChanged;
        public event Action OnSlotsInitialized;

        public void Equip(Weapon.Weapon weapon, WeaponDefinition def)
        {
            int slot = Array.FindIndex(weaponSlots, w => w == null);

            if (slot != -1)
            {
                weaponSlots[slot] = weapon;
            }
            else
            {
                var oldWeapon = weaponSlots[activeSlotIndex];
                if (weaponViews[activeSlotIndex] != null)
                    Destroy(weaponViews[activeSlotIndex].gameObject);
                OnWeaponRemoved?.Invoke(oldWeapon);
                weaponSlots[activeSlotIndex] = weapon;
                slot = activeSlotIndex;
            }

            weapon.SetOwner(GetComponentInParent<Actor>());
            SpawnVisual(slot, def);

            if (weaponSlots[activeSlotIndex] == null)
                activeSlotIndex = slot;

            UpdateWeaponVisibility();
            OnWeaponChanged?.Invoke(ActiveWeapon);
            OnWeaponAdded?.Invoke(weapon);
        }

        
        public void RemoveWeapon(Weapon.Weapon weapon)
        {
            int index = Array.IndexOf(weaponSlots, weapon);
            if (index != -1)
            {
                if (weaponViews[index] != null)
                {
                    if (weaponViews[index].PairedWeapon != null)
                    {
                        Destroy(weaponViews[index].PairedWeapon.gameObject);
                        weaponViews[index].PairedWeapon = null;
                    }

                    Destroy(weaponViews[index].gameObject);
                    weaponViews[index] = null;
                }

                weaponSlots[index] = null;
                OnWeaponRemoved?.Invoke(weapon);
            }
        }

        public bool HasWeapon(string weaponId)
        {
            foreach (var weapon in weaponSlots)
            {
                if (weapon == null)
                    continue;

                if (weapon.definition.weaponId == weaponId)
                    return true;
            }
            return false;
        }
        
        private void SpawnVisual(int slot, WeaponDefinition def)
        {
            if (weaponViews[slot] != null)
            {
                if (weaponViews[slot].PairedWeapon != null)
                    Destroy(weaponViews[slot].PairedWeapon.gameObject);

                Destroy(weaponViews[slot].gameObject);
                weaponViews[slot] = null;
            }

            if (def.bulletsPerShot == 2)
            {
                GameObject leftGO = Instantiate(def.weaponPrefab, leftWeaponHolder.position, leftWeaponHolder.rotation, leftWeaponHolder);
                GameObject rightGO = Instantiate(def.weaponPrefab, rightWeaponHolder.position, rightWeaponHolder.rotation, rightWeaponHolder);

                var leftView = leftGO.GetComponent<WeaponView>();
                var rightView = rightGO.GetComponent<WeaponView>();
                leftView.PairedWeapon = rightView;
                leftView.IsLeadWeapon = true;
                rightView.IsLeadWeapon = false;
                
                weaponViews[slot] = leftView;

                leftView.Bind(weaponSlots[slot] as RangedWeapon);
                rightView.Bind(weaponSlots[slot] as RangedWeapon);
            }
            else
            {
                GameObject go = Instantiate(def.weaponPrefab, mainWeaponHolder.position, mainWeaponHolder.rotation, mainWeaponHolder);
                var view = go.GetComponent<WeaponView>();
                weaponViews[slot] = view;
                view.Bind(weaponSlots[slot] as RangedWeapon);
            }
        }
        
        private void UpdateWeaponVisibility()
        {
            for (int i = 0; i < weaponViews.Length; i++)
            {
                if (weaponViews[i] != null)
                {
                    bool active = i == activeSlotIndex;
                    weaponViews[i].gameObject.SetActive(active);
                    if (weaponViews[i].PairedWeapon != null)
                        weaponViews[i].PairedWeapon.gameObject.SetActive(active);
                }
            }
        }
        
        public void SwitchSlot()
        {
            if (weaponSlots[0] == null || weaponSlots[1] == null)
                return;

            activeSlotIndex = 1 - activeSlotIndex;
            
            UpdateWeaponVisibility();
            OnWeaponChanged?.Invoke(ActiveWeapon);
        }

        private void Update()
        {
            Vector2 cursorWorld = App.Instance.Services.Get<ICoreInputReader>().MouseWorldPosition;

            if (ActiveWeapon != null)
                ActiveWeapon.TargetPosition = cursorWorld;

            ActiveWeapon?.Tick(Time.deltaTime);
        }

        public void Attack()
        {
            ActiveWeapon?.Attack();
        }
        
        public void CancelReloadIfAny()
        {
            if (ActiveWeapon is RangedWeapon ranged)
                ranged.CancelReload();
        }
        
        public void ReloadCurrentWeapon()
        {
            if (ActiveWeapon is RangedWeapon ranged)
                ranged.Reload();
        }
        
        public void InitializeSlots(List<WeaponDefinition> startingWeapons)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                weaponSlots[i] = null;
                if (i < startingWeapons.Count)
                {
                    var weapon = new RangedWeapon(startingWeapons[i]);
                    Equip(weapon, startingWeapons[i]);
                }
            }
            activeSlotIndex = 0;
            UpdateWeaponVisibility();
            OnSlotsInitialized?.Invoke(); 
        }

    }
}