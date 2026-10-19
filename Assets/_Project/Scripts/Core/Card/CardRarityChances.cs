using UnityEngine;

namespace _Project.Scripts.Core.Card
{
    public static class CardRarityChances
    {
        public static CardRarity Roll()
        {
            var roll = Random.value;

            if (roll < 0.6f) return CardRarity.Common; 
            if (roll < 0.9f) return CardRarity.Rare;
            return CardRarity.Epic;
        }
    }
}