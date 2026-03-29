using SEP490G69.Battle.Cards;
using System.Collections.Generic;
using UnityEngine;

namespace SEP490G69.Battle.Combat 
{
    public class RandomSelectCardStrategy : ISelectCardStrategy
    {
        public bool TrySelectCard(BaseBattleCharacterController owner, IReadOnlyList<CardSO> currentDrawPool, out CardSO card)
        {
            card = null;
            if (currentDrawPool == null || currentDrawPool.Count == 0)
            {
                return false;
            }

            card = currentDrawPool[Random.Range(0, currentDrawPool.Count - 1)];
            return true;
        }
    }
}