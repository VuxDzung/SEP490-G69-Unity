namespace SEP490G69
{
    using UnityEngine;

    public enum TextColor
    {
        White = 0,
        Red = 1,
        Yellow = 2,
        Green = 3,
        Grey = 4,
    }

    public static class LoggerUtils
    {
        public static void Logging(string title, string message = "", TextColor titleColor = TextColor.Green, TextColor msgColor = TextColor.White)
        {
            Debug.Log($"<color={GetColor(titleColor)}>-----[{title}]-----</color>\n<color={GetColor(msgColor)}>{message}</color>");
        }

        private static string GetColor(TextColor color)
        {
            return color switch
            {
                TextColor.Red => "red",
                TextColor.Yellow => "yellow",
                TextColor.Green => "green",
                TextColor.Grey => "grey",
                _ => "white",
            };
        }
    }
}