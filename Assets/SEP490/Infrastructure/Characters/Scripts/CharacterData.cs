namespace SEP490G69
{
    using UnityEngine;

    [System.Serializable]
    public class CharacterData 
    {
        public string CharacterID {  get; set; }
        public float StressValue { get; set; }
        public float FatigueValue { get; set; }
    }
}