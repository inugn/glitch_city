using System.Collections.Generic;
using _Project.Scripts.Core.Cards;
using UnityEngine;

namespace _Project.Scripts.Core.Arena
{
    public static class CardGenerator
    {
        private static readonly EffectType[] PositivePool =
        {
            EffectType.DamageMultiplier,
            EffectType.FireRateMultiplier,
            EffectType.MoveSpeedMultiplier,
            EffectType.MaxHealthBonus,
            EffectType.HealInstant
        };

        private static readonly EffectType[] NegativePool =
        {
            EffectType.EnemyDamageMultiplier,
            EffectType.EnemyMaxHealthBonus,
            EffectType.EnemyCountMultiplier
        };

        public static List<CardData> Generate(int count)
        {
            var result = new List<CardData>(count);
            var usedPositives = new HashSet<EffectType>();

            while (result.Count < count)
            {
                var positive = PositivePool[Random.Range(0, PositivePool.Length)];
                if (usedPositives.Contains(positive))
                    continue;

                var negatives = CardEffectCombinations.Map[positive];
                var negative = negatives[Random.Range(0, negatives.Length)];

                usedPositives.Add(positive);
                var rarity = CardRarityChances.Roll();
                
                result.Add(new CardData
                {
                    Rarity = rarity,
                    PositiveEffect = new CardEffect
                    {
                        Type = positive,
                        IsPositive = true,
                        Value = EffectBalance.GetValue(positive, rarity)
                    },
                    NegativeEffect = new CardEffect
                    {
                        Type = negative,
                        IsPositive = false,
                        Value = EffectBalance.GetValue(negative, rarity)
                    }
                });
            }
            return result;
        }
    }
}