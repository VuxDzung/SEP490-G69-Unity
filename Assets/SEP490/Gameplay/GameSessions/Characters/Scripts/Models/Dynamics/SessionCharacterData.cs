using LiteDB;

namespace SEP490G69
{
    [System.Serializable]
    public class SessionCharacterData 
    {
        //-----------------------------------------
        // INDENTIFIER
        //-----------------------------------------
        [BsonId]
        public string Id { get; set; }


        //-----------------------------------------
        // CHARACTER-RUNTIME-STATS
        //-----------------------------------------
        public float CurrentMaxVitality { get; set; }
        public float CurrentPower { get; set; }
        public float CurrentIntelligence { get; set; }
        public float CurrentStamina { get; set; }
        public float CurrentDef {  get; set; }
        public float CurrentAgi { get; set; }
        public float CurrentEnergy { get; set; }
        public float CurrentMood { get; set; }
        public int CurrentRP { get; set; }
    }
}