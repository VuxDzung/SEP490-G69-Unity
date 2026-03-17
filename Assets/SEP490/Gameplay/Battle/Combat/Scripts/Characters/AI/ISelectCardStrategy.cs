using SEP490G69.Battle.Cards;
using System.Collections.Generic;
namespace SEP490G69.Battle.Combat
{
    public interface ISelectCardStrategy
    {
        public bool TrySelectCard(BaseBattleCharacterController owner,
                                  IReadOnlyList<CardSO> currentDrawPool,
                                  out CardSO card);
    }
}
