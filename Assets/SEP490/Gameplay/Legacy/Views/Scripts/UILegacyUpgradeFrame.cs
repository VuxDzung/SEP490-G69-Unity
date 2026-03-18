namespace SEP490G69.Legacy
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UILegacyUpgradeFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private Button m_NextBtn;

        [SerializeField] private Transform m_LegacyStatPrefab;
        [SerializeField] private Transform m_LegacyStatContainer;

        [SerializeField] private TextMeshProUGUI m_LPTmp;
        [SerializeField] private Image m_LegacyIcon;
        [SerializeField] private TextMeshProUGUI m_LegacyStatName;
        [SerializeField] private TextMeshProUGUI m_LegacyDetailsTmp;
        [SerializeField] private TextMeshProUGUI m_LevelStep;
        [SerializeField] private TextMeshProUGUI m_StatGainedTmp;

        [SerializeField] private Button m_UpgradeBtn;
        [SerializeField] private TextMeshProUGUI m_CostTmp;

        private LegacyStatDataHolder _selectedHolder;

        private GameAuthManager _authManager;
        protected GameAuthManager AuthManager
        {
            get
            {
                if (_authManager == null)
                {
                    _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
                }
                return _authManager;
            }
        }
        private GameLegacyController _legacyController;
        protected GameLegacyController LegacyController
        {
            get
            {
                if (_legacyController == null)
                {
                    _legacyController = ContextManager.Singleton.ResolveGameContext<GameLegacyController>();
                }
                return _legacyController;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BackBtn.onClick.AddListener(Back);
            m_UpgradeBtn.onClick.AddListener(UpgradeLegacyStat);

            LoadLegacyStats();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackBtn.onClick.RemoveListener(Back);
            m_UpgradeBtn.onClick.RemoveListener(UpgradeLegacyStat);

            ClearOldUIElements();
        }

        private void LoadLegacyStats()
        {
            ClearOldUIElements();

            string playerId = AuthManager.GetUserId();
            IReadOnlyList<LegacyStatDataHolder> holderUIs = LegacyController.GetAllPlayerLegacyStats(playerId);
            
            foreach (var holder in  holderUIs)
            {
                Transform legacyUITrans = PoolManager.Pools[GameConstants.POOL_UI_LEGACY_STATS].Spawn(m_LegacyStatPrefab, m_LegacyStatContainer);
                UILegacyStatElement legacyUIElement = legacyUITrans.GetComponent<UILegacyStatElement>();
                if (legacyUIElement != null)
                {
                    legacyUIElement.SetOnClickCallback(SelectDetails)
                                   .SetContent(holder.GetRawId(), holder.GetIcon(), holder.GetUpgradeCost(), holder.GetCurrentLevel(), GameConstants.LEGACY_STATS_MAX_LV);
                }
            }

            m_LPTmp.text = LegacyController.GetCurrentLP(playerId).ToString();
        }

        private void ClearOldUIElements()
        {
            if (PoolManager.Pools[GameConstants.POOL_UI_LEGACY_STATS].Count > 0)
            {
                PoolManager.Pools[GameConstants.POOL_UI_LEGACY_STATS].DespawnAll();
            }
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_TITLE);
        }

        private void Next()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_CHAR_SELECT);
        }

        private void SelectDetails(string legacyStatId)
        {
            string playerId = AuthManager.GetUserId();

            _selectedHolder = LegacyController.GetLegacyStatByRawId(playerId, legacyStatId);

            if (_selectedHolder != null)
            {
                m_LegacyStatName.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_LEGACY_STAT_NAMES, _selectedHolder.GetName());
                m_LegacyDetailsTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_LEGACY_STAT_DESCS, _selectedHolder.GetDesc());
                m_LevelStep.text = _selectedHolder.GetCurrentLevel() >= GameConstants.LEGACY_STATS_MAX_LV ? "Max" : $"{_selectedHolder.GetCurrentLevel()} -> {_selectedHolder.GetCurrentLevel() + 1}";
                m_StatGainedTmp.text = _selectedHolder.GetCurrentLevel() >= GameConstants.LEGACY_STATS_MAX_LV ? "Max" : $"{_selectedHolder.GetCurrentValue()} -> {_selectedHolder.GetValue(_selectedHolder.GetCurrentLevel() + 1)}";
                m_LegacyIcon.sprite = _selectedHolder.GetIcon();

                m_CostTmp.text = _selectedHolder.GetCurrentLevel() >= GameConstants.LEGACY_STATS_MAX_LV ? "Max" : $"{_selectedHolder.GetUpgradeCost()} LP";
            }
            else
            {
                m_LegacyStatName.text = string.Empty;
                m_LegacyDetailsTmp.text = string.Empty;
                m_LevelStep.text = string.Empty;
                m_StatGainedTmp.text = string.Empty;
                m_LegacyIcon.sprite = null;
            }
        }

        private void UpgradeLegacyStat()
        {
            if (_selectedHolder == null)
            {
                Debug.Log($"<color=red>[UILegacyUpgradeFrame.UpgradeLegacyStat]</color> No selected data holder is available!");
                return;
            }

            if (LegacyController.TryUpgradeLegacyStat(_selectedHolder.GetEntityId()))
            {
                LoadLegacyStats();
                SelectDetails(_selectedHolder.GetRawId());
            }
        }
    }
}