namespace SEP490G69
{
    public enum EStatusType 
    {
        Life = 0,
        Power = 1,
        Intelligence = 2,
        Defense = 3,
        Agi = 4,
        Vitality = 5,
        Energy = 6,
        Mood = 7,
        Stamina = 8,
        ActionGauge = 9, // When a card has an effect like add/reduce action gauge, the action gauge is applied in the next character turn.
        None
    }
}