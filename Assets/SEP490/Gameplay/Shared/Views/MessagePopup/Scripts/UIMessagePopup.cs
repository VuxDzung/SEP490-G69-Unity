namespace SEP490G69.Shared
{
    using SEP490G69.Addons.Localization;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIMessagePopup : GameUIFrame
    {
        private event Action onConfirm;
        private event Action onCancel;

        [SerializeField] private TextMeshProUGUI m_TitleTmp;
        [SerializeField] private TextMeshProUGUI m_MessageTmp;
        [SerializeField] private Button m_ConfirmBtn;
        [SerializeField] private Button m_CancelBtn;

        private LocalizationManager _localizeManager;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (_localizeManager == null) _localizeManager = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();
            m_ConfirmBtn.onClick.AddListener(Confirm);
            m_CancelBtn.onClick.AddListener(Cancel);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_ConfirmBtn.onClick.RemoveListener(Confirm);
            m_CancelBtn.onClick.RemoveListener(Cancel);
        }

        public void SetContent(string titleId, string messageId, Action onConfirm = null, Action onCancel = null)
        {
            m_TitleTmp.text = _localizeManager.GetText(GameConstants.LOCALIZE_UI_MESSAGE, titleId);
            m_MessageTmp.text = _localizeManager.GetText(GameConstants.LOCALIZE_UI_MESSAGE, messageId);
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;
        }

        private void Confirm()
        {
            onConfirm?.Invoke();
            UIManager.HideFrame(GameConstants.FRAME_ID_MESSAGE_POPUP);
        }
        private void Cancel()
        {
            onCancel?.Invoke();
            UIManager.HideFrame(GameConstants.FRAME_ID_MESSAGE_POPUP);
        }
    }
}