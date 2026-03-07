namespace SEP490G69.Tournament
{
    public class TournamentParticipantData
    {
        public string Id { get; set; }

        public string CharacterId { get; set; }

        public bool IsPlayer { get; set; }

        public int SlotIndex { get; set; }

        public float TotalStats { get; set; }

        public bool IsEliminated { get; set; }
    }
}