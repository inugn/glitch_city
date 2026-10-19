using System.Collections.Generic;
using _Project.Scripts.Core.Arena;

namespace _Project.Scripts.Core.Cards
{
    public static class CardEffectCombinations
    {
        public static readonly Dictionary<EffectType, EffectType[]> Map =
            new()
            {
                {
                    EffectType.DamageMultiplier,
                    new[] { EffectType.EnemyMaxHealthBonus }
                },
                {
                    EffectType.ReloadSpeedMultiplier,
                    new[] { EffectType.EnemyDamageMultiplier }
                },
                {
                    EffectType.FireRateMultiplier,
                    new[] { EffectType.EnemyDamageMultiplier, EffectType.EnemyMaxHealthBonus }
                },
                {
                    EffectType.MaxHealthBonus,
                    new[] { EffectType.EnemyCountMultiplier, EffectType.EnemyDamageMultiplier }
                },
                {
                    EffectType.HealInstant,
                    new[] { EffectType.EnemyCountMultiplier }
                },
                {
                    EffectType.MoveSpeedMultiplier,
                    new[] { EffectType.EnemyCountMultiplier }
                }
            };
    }
}