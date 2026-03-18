namespace SEP490G69.Legacy
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UILegacyStatElement : MonoBehaviour, IPooledObject
    {
        private Action<string> _onClick;

        [SerializeField] private Button m_btnRef;
        [SerializeField] private Image m_StatIcon;
        [SerializeField] private TextMeshProUGUI m_CostTmp;
        [SerializeField] private TextMeshProUGUI m_LevelTmp;
        [SerializeField] private List<Transform> m_LevelTrans;

        private string _playerLegacyStatId;

        public void Spawn()
        {
            m_btnRef.onClick.AddListener(Click);
        }
        public void Despawn()
        {
            m_btnRef.onClick.RemoveListener(Click);
        }

        public UILegacyStatElement SetOnClickCallback(Action<string> onClick)
        {
            _onClick = onClick;
            return this;
        }

        public void SetContent(string legacyStatId, Sprite icon, int cost, int currentLevel, int maxLv)
        {
            _playerLegacyStatId = legacyStatId;
            if (m_StatIcon != null) m_StatIcon.sprite = icon;
            if (m_CostTmp != null) m_CostTmp.text = $"{(currentLevel >= GameConstants.LEGACY_STATS_MAX_LV ? "Max" : cost.ToString() + " LP")}";
            if (m_LevelTmp != null) m_LevelTmp.text = $"Lv {currentLevel}/{maxLv}";

            m_LevelTrans.ForEach(l => l.gameObject.SetActive(false));

            for (int i = 0; i < currentLevel; i++)
            {
                m_LevelTrans[i].gameObject.SetActive(true);
            }
        }

        private void Click()
        {
            _onClick?.Invoke(_playerLegacyStatId);
        }
    }
}