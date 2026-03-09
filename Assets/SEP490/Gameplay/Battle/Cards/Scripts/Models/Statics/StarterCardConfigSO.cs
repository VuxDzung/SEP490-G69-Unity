namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu]
    public class StarterCardConfigSO : ScriptableObject
    {
        [SerializeField] private List<StarterCardData> m_StarterCards;

        public IReadOnlyList<StarterCardData> StarterCards => m_StarterCards;
    }

    [System.Serializable]
    public class StarterCardData
    {
        public string cardId;
        public int amount;
    }
}