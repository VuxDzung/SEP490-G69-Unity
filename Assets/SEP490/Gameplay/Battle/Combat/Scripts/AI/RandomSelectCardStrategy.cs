using SEP490G69.Battle.Cards;
using System.Collections.Generic;
using UnityEngine;

namespace SEP490G69.Battle.Combat 
{
    public class RandomSelectCardStrategy : MonoBehaviour, ISelectCardStrategy
    {
        public bool TrySelectCard(CharacterDataHolder readonlyDataHolder, CharacterDataHolder runtimeDataHolder, IReadOnlyList<CardSO> currentDrawPool, out CardSO card)
        {
            card = currentDrawPool[Random.Range(0, currentDrawPool.Count - 1)];
            return true;
        }
    }
}