namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using UnityEngine;

    public class CharacterCardProcessor : MonoBehaviour
    {
        private BaseBattleCharacterController _owner;

        private List<CardSO> _deck = new();
        private List<CardSO> _discard = new();
        private List<CardSO> _hand = new();

        private CardSO _selected;

        public void SetOwner(BaseBattleCharacterController owner)
        {
            _owner = owner;
        }

        public void ExecuteSelectedCard(BaseBattleCharacterController target)
        {
            if (_selected == null) return;

            var runtimeCard = CardFactory.Create(_selected);

            runtimeCard.Execute(_owner, target);
        }

        public void SelectCard(CardSO card)
        {
            _selected = card;
        }
    }
}