using _Project.Scripts.Core.Arena;
using UnityEngine;

namespace _Project.Scripts.Core.Cards
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