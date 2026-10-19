using System;
using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Core.Cards;

namespace _Project.Scripts.Core.Arena
{
    public sealed class CardChoiceController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private CardView cardPrefab;

        private Action<CardData> onChosen;
        private readonly List<CardView> spawned = new();

        public void Open(Action<CardData> onCardChosen)
        {
            onChosen = onCardChosen;

            Clear();
            SpawnCards();
        }

        private void SpawnCards()
        {
            var cards = CardGenerator.Generate(3);

            foreach (var card in cards)
            {
                var view = Instantiate(cardPrefab, cardsContainer);

                view.Setup(
                    card.Title,
                    GetRarityColor(card.Rarity),
                    card.PositiveEffect.GetDescription(),
                    card.NegativeEffect.GetDescription(),
                    () => Choose(card)
                );
            }
        }

        private void Choose(CardData card)
        {
            onChosen?.Invoke(card);
        }

        private void Clear()
        {
            for (int i = cardsContainer.childCount - 1; i >= 0; i--)
                Destroy(cardsContainer.GetChild(i).gameObject);
        }

        private Color GetRarityColor(CardRarity rarity)
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