using System;
using _Project.Scripts.Core.Arena;

namespace _Project.Scripts.Core.Card
{
    [Serializable]
    public class CardData
    {
        public CardRarity Rarity;

        public CardEffect PositiveEffect;
        public CardEffect NegativeEffect;

        public string Title =>
            Rarity switch
            {
                CardRarity.Common => "Обычная",
                CardRarity.Rare => "Редкая",
                CardRarity.Epic => "Эпическая",
                _ => "Карта"
            };
    }
}