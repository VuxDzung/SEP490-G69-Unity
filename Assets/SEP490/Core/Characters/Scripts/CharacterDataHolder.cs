namespace SEP490G69
{
    using UnityEngine;

    public class CharacterDataHolder 
    {
        public CharacterData Data {  get; private set; }
        public CharacterDataSO SO { get; private set; }

        public CharacterDataHolder(CharacterData data, CharacterDataSO sO)
        {
            Data = data;
            SO = sO;
        }


    }
}