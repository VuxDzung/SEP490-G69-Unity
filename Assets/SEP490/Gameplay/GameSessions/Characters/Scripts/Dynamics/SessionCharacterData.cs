namespace SEP490G69
{
    [System.Serializable]
    public class SessionCharacterData 
    {
        //-----------------------------------------
        // INDENTIFIER
        //-----------------------------------------
        public string SessionId { get; set; }
        public string CharacterId { get; set; }


        //-----------------------------------------
        // CHARACTER-RUNTIME-STATS
        //-----------------------------------------
        public int CurrentMaxVitality { get; set; }
        public int CurrentPower { get; set; }
        public int CurrentIntelligence { get; set; }
        public float CurrentStamina { get; set; }
        public float CurrentDef {  get; set; }
        public float CurrentAgi { get; set; }
        public float CurrentEnergy { get; set; }
        public float CurrentMood { get; set; }
        public int CurrentRP { get; set; }
    }
}