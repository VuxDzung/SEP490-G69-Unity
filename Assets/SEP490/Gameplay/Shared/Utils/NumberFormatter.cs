namespace SEP490G69
{
    using UnityEngine;

    public class NumberFormatter
    {
        public static string FormatGold(long value)
        {
            if (value >= 1_000_000_000)
                return (value / 1_000_000_000f).ToString("0.#") + "B";

            if (value >= 1_000_000)
                return (value / 1_000_000f).ToString("0.#") + "M";

            if (value >= 1_000)
                return (value / 1_000f).ToString("0.#") + "K";

            return value.ToString();
        }
    }
}