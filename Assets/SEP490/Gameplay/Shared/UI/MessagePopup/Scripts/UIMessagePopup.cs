namespace SEP490G69.Shared
{
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
        [SerializeField] private string m_DefaultConfirmMsg;
        [SerializeField] private string m_DefaultCancelMsg;
        [SerializeField] private Button m_ConfirmBtn;
        [SerializeField] private Button m_CancelBtn;
        [SerializeField] private TextMeshProUGUI m_ConfirmTmp;
        [SerializeField] private TextMeshProUGUI m_CancelTmp;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_ConfirmBtn.onClick.AddListener(Confirm);
            m_CancelBtn.onClick.AddListener(Cancel);
            SetOptionMessage();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_ConfirmBtn.onClick.RemoveListener(Confirm);
            m_CancelBtn.onClick.RemoveListener(Cancel);
        }

        public UIMessagePopup SetContent(string titleId, string messageId, bool hasConfirm = true, bool hasCancel = true, Action onConfirm = null, Action onCancel = null)
        {
            m_TitleTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_UI_MESSAGE, titleId);
            m_MessageTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_UI_MESSAGE, messageId);

            this.onConfirm = onConfirm;
            this.onCancel = onCancel;

            m_ConfirmBtn.gameObject.SetActive(hasConfirm);
            m_CancelBtn.gameObject.SetActive(hasCancel);

            return this;
        }

        public UIMessagePopup SetOptionMessage(string confirmIdMsg = "", string cancelIdMsg = "")
        {
            string confirmMsg = LocalizeManager.GetText(GameConstants.LOCALIZE_UI_MESSAGE, string.IsNullOrEmpty(confirmIdMsg) ? m_DefaultConfirmMsg : confirmIdMsg);
            string cancelMsg = LocalizeManager.GetText(GameConstants.LOCALIZE_UI_MESSAGE, string.IsNullOrEmpty(cancelIdMsg) ? m_DefaultConfirmMsg : cancelIdMsg);

            m_ConfirmTmp.text = confirmMsg;
            m_CancelTmp.text = cancelMsg;

            return this;
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