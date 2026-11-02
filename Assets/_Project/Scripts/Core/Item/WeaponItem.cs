using System;
using _Project.Scripts.Core;
using _Project.Scripts.Core.Item;
using _Project.Scripts.Player;
using _Project.Scripts.Weapon;
using UnityEngine;

namespace _Project.Scripts.Items
{
    public sealed class WeaponItem : Item
    {
        [Header("Weapon Data")]
        [SerializeField] private WeaponDefinition weaponDefinition;

        [Header("UI")]
        [SerializeField] private ItemView view;

        [Header("Preview")]
        [SerializeField] private Transform visualRoot;

        private Player.Player trackedPlayer;
        private WeaponView previewView;

        private MetaProgress metaProgress;

        private bool isAvailable = true;

        // чтобы корректно отписываться (нельзя отписаться от лямбды без сохранения ссылки)
        private Action slotsInitHandler;

        protected override void Awake()
        {
            base.Awake();

            metaProgress = App.Instance?.Services?.Get<MetaProgress>();
            if (metaProgress == null)
                Debug.LogWarning("[WeaponItem] MetaProgress not found. Level lock will fallback to level=1.", this);

            trackedPlayer = FindObjectOfType<Player.Player>();
            if (trackedPlayer == null)
            {
                // В стартзоне игрок может появиться позже — но ты сейчас ищешь в Awake.
                // Если захочешь — можно расширить и делать lazy-resolve в Update/OnTriggerEnter.
                return;
            }

            slotsInitHandler = OnSlotsInitializedInternal;
            trackedPlayer.OnSlotsInitialized += slotsInitHandler;
        }

        private void OnDestroy()
        {
            if (trackedPlayer != null)
            {
                if (slotsInitHandler != null)
                    trackedPlayer.OnSlotsInitialized -= slotsInitHandler;

                if (trackedPlayer.Combat != null)
                {
                    trackedPlayer.Combat.OnWeaponAdded -= HandleWeaponChange;
                    trackedPlayer.Combat.OnWeaponRemoved -= HandleWeaponChange;
                }
            }
            
        }

        private void OnSlotsInitializedInternal()
        {
            UpdateAvailability();

            if (trackedPlayer != null && trackedPlayer.Combat != null)
            {
                trackedPlayer.Combat.OnWeaponAdded += HandleWeaponChange;
                trackedPlayer.Combat.OnWeaponRemoved += HandleWeaponChange;
            }
            
        }

        private void OnPlayerLevelChanged(int _)
        {
            UpdateAvailability();
        }

        private void ApplyView()
        {
            if (weaponDefinition == null || view == null)
                return;

            view.SetName(weaponDefinition.weaponId);
            view.SetIcon(weaponDefinition.icon);
        }

        private void SpawnPreview()
        {
            if (weaponDefinition == null || weaponDefinition.weaponPrefab == null)
                return;

            Transform parent = visualRoot != null ? visualRoot : transform;

            var go = Instantiate(weaponDefinition.weaponPrefab, parent);

            previewView = go.GetComponent<WeaponView>();
            if (previewView != null)
                previewView.SetAsPreview();
        }

        public void SetDefinition(WeaponDefinition definition)
        {
            weaponDefinition = definition;
            ApplyView();
            SpawnPreview();

            // если игрок уже есть и слоты уже инициализированы — обновим сразу
            UpdateAvailability();
        }

        private void UpdateAvailability()
        {
            if (weaponDefinition == null)
                return;

            if (trackedPlayer == null || trackedPlayer.Combat == null)
            {
                // Игрок ещё не найден/не готов — не блокируем предмет жестко, но и не показываем интеракт.
                SetAvailable(false);
                return;
            }

            bool hasWeapon = trackedPlayer.Combat.HasWeapon(weaponDefinition.weaponId);

            int playerLevel = metaProgress != null ? metaProgress.PlayerLevel : 1;
            int requiredLevel = Mathf.Max(1, weaponDefinition.requiredLevel);
            bool levelOk = playerLevel >= requiredLevel;

            // доступно только если НЕ уже есть оружие и уровень достаточный
            isAvailable = !hasWeapon && levelOk;

            SetAvailable(isAvailable);

            // если хочешь — можно тут выводить причину в UI (если ItemView поддерживает текст)
            // пока оставлю только отладочный лог при нахождении рядом
            if (!levelOk && IsPlayerNearby)
                Debug.Log($"[WeaponItem] Locked: need level {requiredLevel}, player level {playerLevel}. WeaponId={weaponDefinition.weaponId}", this);
        }

        private void SetAvailable(bool value)
        {
            isAvailable = value;

            if (view != null)
                view.SetDisabled(!value);

            if (previewView != null)
                previewView.SetDisabled(!value);

            if (InteractHint != null)
                InteractHint.SetActive(value && IsPlayerNearby);
        }

        public override void Interact(Player.Player player)
        {
            if (!isAvailable)
                return;

            if (player == null || weaponDefinition == null)
                return;

            player.Combat.Equip(
                new RangedWeapon(weaponDefinition),
                weaponDefinition
            );

            UpdateAvailability();
        }

        protected override void OnPlayerEnter(Collider2D other)
        {
            base.OnPlayerEnter(other);
            UpdateAvailability();
        }

        protected override void OnPlayerExit(Collider2D other)
        {
            if (InteractHint != null)
                InteractHint.SetActive(false);

            base.OnPlayerExit(other);
        }

        private void HandleWeaponChange(Weapon.Weapon _)
        {
            UpdateAvailability();
        }
    }
}
