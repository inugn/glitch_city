using _Project.Scripts.Items;
using UnityEngine;

namespace _Project.Scripts.Core.Item
{
    public sealed class MedkitItem : Item
    {
        [Header("Medkit Data")]
        [SerializeField] private MedkitDefinition definition;

        [Header("UI")]
        [SerializeField] private MedkitItemView view;

        private GameSession session;
        private Player.Player player;

        private bool isAvailable;

        protected override void Awake()
        {
            base.Awake();

            session = App.Instance.Services.Get<GameSession>();
            player = FindObjectOfType<Player.Player>();

            ApplyView();
            UpdateAvailability();
        }

        private void ApplyView()
        {
            if (definition == null || view == null)
                return;

            view.SetName($"{definition.medkitId}");
            view.SetIcon(definition.icon);
            view.SetHeal($"Восстанавливает {definition.healAmount} HP");
            view.SetPrice($"{definition.price}");
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

        private void UpdateAvailability()
        {
            if (definition == null || player == null || session == null)
            {
                SetAvailable(false);
                return;
            }

            bool hpFull = player.Stats.Health.CurrentHealth >= player.Stats.Health.MaxHealth;
            bool hasCredits = session.Credits >= definition.price;

            isAvailable = !hpFull && hasCredits;

            SetAvailable(isAvailable);
        }

        private void SetAvailable(bool value)
        {
            if (view != null)
                view.SetDisabled(!value);

            if (InteractHint != null)
                InteractHint.SetActive(value && IsPlayerNearby);
        }

        public override void Interact(Player.Player player)
        {
            if (!isAvailable || definition == null)
                return;

            // лечим
            player.Stats.Health.Heal(definition.healAmount);

            // списываем кредиты
            session.AddCredits(-definition.price);

            UpdateAvailability();
        }
    }
}