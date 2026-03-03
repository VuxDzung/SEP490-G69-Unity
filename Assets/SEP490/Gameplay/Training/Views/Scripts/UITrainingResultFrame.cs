namespace SEP490G69.Training
{
    using SEP490G69.Addons.Localization;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITrainingResultFrame : GameUIFrame
    {
        [Header("General UI")]
        [SerializeField] private TextMeshProUGUI m_TitleTmp;
        [SerializeField] private Button m_CloseBtn;

        [Header("Stat Modifiers")]
        [SerializeField] private UIStatModifier m_VitModifier;
        [SerializeField] private UIStatModifier m_PowModifier;
        [SerializeField] private UIStatModifier m_AgiModifier;
        [SerializeField] private UIStatModifier m_IntModifier;
        [SerializeField] private UIStatModifier m_StaModifier;
        private Dictionary<EStatusType, UIStatModifier> _statMap = null;

        #region Properties (Lazy getters)
        private LocalizationManager _localizeManager;
        protected LocalizationManager LocalizeManager
        {
            get
            {
                if (this._localizeManager == null)
                {
                    this._localizeManager = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();
                }
                return this._localizeManager;
            }
        }
        private EventManager _eventManager;
        protected EventManager EventManager
        {
            get
            {
                if (_eventManager == null)
                {
                    _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
                }
                return _eventManager;
            }
        }
        #endregion

        #region Allocate & Deallocate
        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_CloseBtn.onClick.AddListener(CloseFrame);

            if (_statMap == null || _statMap.Count == 0)
            {
                _statMap = new Dictionary<EStatusType, UIStatModifier>
                {
                    { EStatusType.Vitality, m_VitModifier },
                    { EStatusType.Power, m_PowModifier },
                    { EStatusType.Agi, m_AgiModifier },
                    { EStatusType.Intelligence, m_IntModifier },
                    { EStatusType.Stamina, m_StaModifier },
                };
            }
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_CloseBtn.onClick.RemoveListener(CloseFrame);
        }
        #endregion

        #region Setters
        public void SetResult(string title, TrainingResult result)
        {
            SetTitle(title);

            foreach (var modifier in _statMap.Values)
                modifier.Hide();

            foreach (var change in result.Changes)
            {
                if (!_statMap.TryGetValue(change.StatusType, out UIStatModifier ui))
                    continue;

                ui.Show();
                ui.SetValue(change.Before, change.After);
            }
        }

        public void SetTitle(string title)
        {
            m_TitleTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_EXERCISE_NAMES, title);
        }

        public void SetVitModifier(float before, float after)
        {
            m_VitModifier.SetValue(before, after);
        }

        public void SetPowModifier(float before, float after)
        {
            m_PowModifier.SetValue(before, after);
        }

        public void SetAgiModifier(float before, float after)
        {
            m_AgiModifier.SetValue(before, after);
        }

        public void SetIntModifier(float before, float after)
        {
            m_IntModifier.SetValue(before, after);
        }

        public void SetStaModifier(float before, float after)
        {
            m_StaModifier.SetValue(before, after);
        }
        #endregion

        #region Actions
        private void CloseFrame()
        {
            UIManager.HideFrame(FrameId);
            EventManager.Publish<TrainingCompletedEvent>(new TrainingCompletedEvent());
        }
        #endregion
    }
}