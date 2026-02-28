namespace SEP490G69.Battle.Combat
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIBattleFrame : GameUIFrame
    {
        [SerializeField] private Transform m_CardsContainer;
        [SerializeField] private Button m_RestBtn;
        [SerializeField] private Button m_ActionBtn;

        private CharacterCombatController _playerCharCombat;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
        }
        
        private void LoadAvailableCards()
        {
            if (m_CardsContainer == null)
            {
                return;
            }
        }

        private void Rest()
        {

        }

        public void SetCharacter(CharacterCombatController character)
        {
            _playerCharCombat = character;
        }
    }
}