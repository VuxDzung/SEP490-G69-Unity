namespace SEP490G69.Battle.Combat
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICombatDetailsFrame : BaseCombatFrame
    {
        [Header("Player Stats")]
        [SerializeField] private UITextSlider m_PlayerVitSlider;
        [SerializeField] private UITextSlider m_PlayerPowSlider;
        [SerializeField] private UITextSlider m_PlayerAgiSlider;
        [SerializeField] private UITextSlider m_PlayerIntSlider;
        [SerializeField] private UITextSlider m_PlayerStaSlider;
        [SerializeField] private Button m_ChangeDeckBtn;
        [SerializeField] private Button m_ChangeRelicBtn;
        [SerializeField] private Button m_PlayerDetailsBtn;

        [Header("Enemy Stats")]
        [SerializeField] private UITextSlider m_EnemyVitSlider;
        [SerializeField] private UITextSlider m_EnemyPowSlider;
        [SerializeField] private UITextSlider m_EnemyAgiSlider;
        [SerializeField] private UITextSlider m_EnemyIntSlider;
        [SerializeField] private UITextSlider m_EnemyStaSlider;
        [SerializeField] private Button m_ViewDeckBtn;
        [SerializeField] private Button m_ViewEnemyDetailsBtn;

        [Header("Combat Details")]
        [SerializeField] private TextMeshProUGUI m_TournamentNameTmp;
        [SerializeField] private TextMeshProUGUI m_CombatNameTmp;
        [SerializeField] private TextMeshProUGUI m_PlayerCharNameTmp;
        [SerializeField] private TextMeshProUGUI m_EnemyNameTmp;
        [SerializeField] private Button m_StartBattleBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();

            m_ChangeDeckBtn.onClick.AddListener(ChangePlayerDeck);
            m_ChangeRelicBtn.onClick.AddListener(ChangeRelic);
            m_PlayerDetailsBtn.onClick.AddListener(ViewPlayerCharDetails);


            m_StartBattleBtn.onClick.AddListener(StartBattle);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_ChangeDeckBtn.onClick.RemoveListener(ChangePlayerDeck);
            m_ChangeRelicBtn.onClick.RemoveListener(ChangeRelic);
            m_PlayerDetailsBtn.onClick.RemoveListener(ViewPlayerCharDetails);
            m_StartBattleBtn.onClick.RemoveListener(StartBattle);
        }

        private void ChangePlayerDeck()
        {

        }

        private void ChangeRelic()
        {

        }

        private void ViewPlayerCharDetails()
        {

        }

        private void StartBattle()
        {
            CombatController.StartBattle();
        }

        #region Player Stats
        public UICombatDetailsFrame SetPlayerCharName(string name)
        {
            m_PlayerCharNameTmp.text = name;
            return this;
        }
        public UICombatDetailsFrame SetPlayerVit(float cur, float max)
        {
            m_PlayerVitSlider.SetValue(cur, max);
            m_PlayerVitSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        public UICombatDetailsFrame SetPlayerPow(float cur, float max)
        {
            m_PlayerPowSlider.SetValue(cur, max);
            m_PlayerPowSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        public UICombatDetailsFrame SetPlayerAgi(float cur, float max)
        {
            m_PlayerAgiSlider.SetValue(cur, max);
            m_PlayerAgiSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        public UICombatDetailsFrame SetPlayerInt(float cur, float max)
        {
            m_PlayerIntSlider.SetValue(cur, max);
            m_PlayerIntSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        public UICombatDetailsFrame SetPlayerSta(float cur, float max)
        {
            m_PlayerStaSlider.SetValue(cur, max);
            m_PlayerStaSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        #endregion

        #region Enemy Stats
        public UICombatDetailsFrame SetEnemyName(string enemyName)
        {
            m_EnemyNameTmp.text = enemyName;
            return this;
        }
        public UICombatDetailsFrame SetEnemyVit(float cur, float max)
        {
            m_EnemyVitSlider.SetValue(cur, max);
            m_EnemyVitSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        public UICombatDetailsFrame SetEnemyPow(float cur, float max)
        {
            m_EnemyPowSlider.SetValue(cur, max);
            m_EnemyPowSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        public UICombatDetailsFrame SetEnemyAgi(float cur, float max)
        {
            m_EnemyAgiSlider.SetValue(cur, max);
            m_EnemyAgiSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        public UICombatDetailsFrame SetEnemyInt(float cur, float max)
        {
            m_EnemyIntSlider.SetValue(cur, max);
            m_EnemyIntSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        public UICombatDetailsFrame SetEnemySta(float cur, float max)
        {
            m_EnemyStaSlider.SetValue(cur, max);
            m_EnemyStaSlider.SetRank(CharacterStatUtils.GetStatRank(cur));
            return this;
        }

        #endregion
    }
}