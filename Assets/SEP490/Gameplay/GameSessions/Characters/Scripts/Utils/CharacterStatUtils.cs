namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CharacterStatUtils
    {
        public const int MAX_STAT_VALUE = 1000;

        private static readonly Dictionary<int, string> _statusRankLookUp = new Dictionary<int, string>
        {
            { 100, "D" },
            { 200, "D+" },
            { 300, "C" },
            { 400, "C+" },
            { 500, "B" },
            { 600, "B+" },
            { 700, "A" },
            { 800, "A+" },
            { 900, "S+" },
            { 1000, "SS" },
        };

        private static readonly Dictionary<int, string> _reputationRankLookup = new Dictionary<int, string>
        {
            { 99, "F" },
            { 299, "E" },
            { 599, "D" },
            { 999, "C" },
            { 1499, "B" },
            { 2499, "A" },
            { int.MaxValue, "S" }
        };

        private static readonly Dictionary<int, string> _moodRankLookUp = new Dictionary<int, string>
        {
            { 100, "Great" },
            { 79, "Good" },
            { 59, "Neutral" },
            { 39, "Bad" },
            { 19, "Awful" },
        };

        public static string GetStatRank(float value)
        {
            value = Mathf.Clamp(value, 0, 1000);

            foreach (var pair in _statusRankLookUp.OrderBy(x => x.Key))
            {
                if (value <= pair.Key)
                {
                    return pair.Value;
                }
            }

            return "SS";
        }

        public static string GetMoodRank(float value)
        {
            value = Mathf.Clamp(value, 0, 100);

            foreach (var pair in _moodRankLookUp.OrderByDescending(x => x.Key))
            {
                if (value >= pair.Key)
                {
                    return pair.Value;
                }
            }

            return "Awful";
        }

        public static string GetReputationRank(int reputation)
        {
            reputation = Mathf.Max(0, reputation);

            foreach (var pair in _reputationRankLookup.OrderBy(x => x.Key))
            {
                if (reputation <= pair.Key)
                {
                    return pair.Value;
                }
            }

            return "S";
        }

        public static int GetReputationRankMax(int reputation)
        {
            reputation = Mathf.Max(0, reputation);

            foreach (var pair in _reputationRankLookup.OrderBy(x => x.Key))
            {
                if (reputation <= pair.Key)
                {
                    return pair.Key;
                }
            }

            return int.MaxValue;
        }

        public static int GetStatRankMaxValue(float value)
        {
            value = Mathf.Clamp(value, 0, 1000);

            foreach (var pair in _statusRankLookUp.OrderBy(x => x.Key))
            {
                if (value <= pair.Key)
                {
                    return pair.Key;
                }
            }

            return 1000;
        }
    }
}