using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Card;
using UnityEngine;

namespace _Project.Scripts.Core.Arena
{
    public sealed class CardChoiceController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private CardView cardPrefab;

        private Action<CardData> onChosen;

        // Пул UI-элементов, чтобы не Destroy/Instantiate каждый раз
        private readonly List<CardView> spawned = new(4);

        public void Open(IReadOnlyList<CardData> cards, Action<CardData> onCardChosen)
        {
            if (cardsContainer == null || cardPrefab == null)
            {
                Debug.LogError("[CardChoiceController] cardsContainer/cardPrefab is not assigned.", this);
                return;
            }

            onChosen = onCardChosen;

            EnsurePoolSize(cards != null ? cards.Count : 0);
            Bind(cards);
        }

        public void Close()
        {
            onChosen = null;
            HideAll();
        }

        private void EnsurePoolSize(int count)
        {
            while (spawned.Count < count)
            {
                var view = Instantiate(cardPrefab, cardsContainer);
                view.gameObject.SetActive(false);
                spawned.Add(view);
            }
        }

        private void Bind(IReadOnlyList<CardData> cards)
        {
            int count = cards != null ? cards.Count : 0;

            for (int i = 0; i < spawned.Count; i++)
            {
                var view = spawned[i];

                if (i >= count)
                {
                    view.gameObject.SetActive(false);
                    continue;
                }

                var card = cards[i]; // локальная переменная — без сюрпризов с замыканиями
                view.gameObject.SetActive(true);

                view.Setup(
                    card.Title,
                    GetRarityColor(card.Rarity),
                    card.PositiveEffect != null ? card.PositiveEffect.GetDescription() : string.Empty,
                    card.NegativeEffect != null ? card.NegativeEffect.GetDescription() : string.Empty,
                    () => Choose(card)
                );
            }
        }

        private void HideAll()
        {
            for (int i = 0; i < spawned.Count; i++)
                spawned[i].gameObject.SetActive(false);
        }

        private void Choose(CardData card)
        {
            onChosen?.Invoke(card);
        }

        private static Color GetRarityColor(CardRarity rarity)
        {
            return rarity switch
            {
                CardRarity.Common => Color.gray,
                CardRarity.Rare => new Color(0.3f, 0.5f, 1f),
                CardRarity.Epic => new Color(0.7f, 0.3f, 1f),
                _ => Color.white
            };
        }
    }
}
