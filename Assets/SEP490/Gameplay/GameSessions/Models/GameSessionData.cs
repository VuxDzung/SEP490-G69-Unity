namespace SEP490G69
{
    public class GameSessionData 
    {
        public string SessionId { get; set; }
        public string PlayerId { get; set;  }

        public bool IsCompleted { get; set; }
        public int CurrentWeek { get; set; }
        public int CurrentGold { get; set; }
    }
}