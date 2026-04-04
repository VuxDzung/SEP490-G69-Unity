namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using System.Collections.Generic;
    using UnityEngine;

    public class QuietCalculationCard : BaseCard
    {
        public QuietCalculationCard(CardSO data) : base(data)
        {
        }

        public override void Execute(PlayerActorController source, BaseCombatActor opponent)
        {
            source.CardsService.DiscardCurrentDraw();
        }
    }
}