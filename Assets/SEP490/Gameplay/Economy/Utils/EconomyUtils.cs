using System.Collections.Generic;
using System;
using System.Linq;

namespace SEP490G69.Economy
{
    public class EconomyUtils
    {
        public const int CONSUMABLE_COUNT = 3;
        public const int RELIC_COUNT = 3;

        public static float ConvertRarityToPercent(ERarityType rarity)
        {
            return rarity switch
            {
                ERarityType.D => 0.5f,
                ERarityType.C => 0.3f,
                ERarityType.B => 0.1f,
                ERarityType.A => 0.06f,
                ERarityType.S => 0.03f,
                ERarityType.SS => 0.01f,
                _ => 1f
            };
        }

        public static T GetRandomByWeight<T>(List<T> items, Func<T, float> weightSelector)
        {
            float totalWeight = items.Sum(weightSelector);
            float randomPoint = UnityEngine.Random.value * totalWeight;

            float current = 0f;

            foreach (var item in items)
            {
                current += weightSelector(item);
                if (randomPoint <= current)
                    return item;
            }

            return items.Last();
        }

        public static List<T> GetRandomUniqueByWeight<T>(List<T> source, int count, Func<T, float> weightSelector)
        {
            List<T> pool = new List<T>(source);
            List<T> result = new();

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                var item = GetRandomByWeight(pool, weightSelector);
                result.Add(item);
                pool.Remove(item); // tránh trùng
            }

            return result;
        }
    }
}