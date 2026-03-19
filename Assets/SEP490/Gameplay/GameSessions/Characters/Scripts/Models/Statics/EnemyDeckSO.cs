namespace SEP490G69
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu (fileName = "Deck_")]
    public class EnemyDeckSO : ScriptableObject
    {
        [SerializeField] private List<string> cardIdList;

        public IReadOnlyList<string> CardIdList => cardIdList;
    }
}