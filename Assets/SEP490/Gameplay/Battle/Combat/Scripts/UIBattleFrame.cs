namespace SEP490G69.Battle.Combat
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIBattleFrame : GameUIFrame
    {
        [SerializeField] private Transform m_CardsContainer;
        [SerializeField] private Button m_RestBtn;
        [SerializeField] private Button m_ActionBtn;

        private PlayerCombatController _playerCharController;
        private EnemyCombatController _enemyController;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_RestBtn.onClick.AddListener(Rest);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_RestBtn.onClick.RemoveListener(Rest);
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

        public UIBattleFrame SetPlayerController(PlayerCombatController character)
        {
            _playerCharController = character;
            return this;
        }
        public UIBattleFrame SetEnemyController(EnemyCombatController character)
        {
            _enemyController = character;
            return this;
        }
    }
}