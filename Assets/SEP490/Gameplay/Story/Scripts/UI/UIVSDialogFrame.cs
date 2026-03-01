namespace SEP490G69
{
    using System.Collections;
    using SEP490G69.Addons.Localization;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIVSDialogFrame : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI m_SpeakerNameTmp;
        [SerializeField] private Image m_Image;
        [SerializeField] private TextMeshProUGUI m_DialogTmp;
        [SerializeField] private Button m_NextBtn;
        [SerializeField] private Button m_AutoBtn;
        [SerializeField] private Button m_SkipBtn;
        [SerializeField] private GameObject m_ChoiceRoot;
        [SerializeField] private GameObject m_ChoiceContainer;
        [SerializeField] private Transform m_ChoicePrefab;
        [SerializeField] private Image m_BgImage;

        [SerializeField] private float m_CharInterval = 0.03f;

        private LocalizationManager _localization;
        private CharacterConfigSO _characterConfig;
        private EventManager _eventManager;

        private Coroutine _typingCoroutine;
        private string _fullDialogText;
        private bool _isTyping;
        private bool _autoMode;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (_eventManager ==null) _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
            if (_localization == null) _localization = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();
            if (_characterConfig == null) _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
            _eventManager.Subscribe<SelectChoice>(DispatchChoiceSelection);
            m_NextBtn.onClick.AddListener(Next);
            m_AutoBtn.onClick.AddListener(Auto);
            m_SkipBtn.onClick.AddListener(Skip);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_NextBtn.onClick.RemoveListener(Next);
            m_AutoBtn.onClick.RemoveListener(Auto);
            m_SkipBtn.onClick.RemoveListener(Skip);
            _eventManager.Unsubscribe<SelectChoice>(DispatchChoiceSelection);
            ClearChoices();
        }

        public UIVSDialogFrame RenderDialog(string speakerID, string dialogID, Sprite bgSprite)
        {
            string dialog = _localization.GetText(GameConstants.LOCALIZE_CATEGORY_DIALOG, dialogID);

            if (bgSprite != null)
            {
                m_BgImage.sprite = bgSprite;
                m_BgImage.enabled = true;
            }
            else
            {
                m_BgImage.sprite = null;
                m_BgImage.enabled = false;
            }

            if (dialog.Contains("<USER_NAME>"))
            {
                string playerName = "Player 1";
                dialog = dialog.Replace("<USER_NAME>", playerName);
            }

            BaseCharacterSO character = _characterConfig.GetCharacterById(speakerID);
            m_SpeakerNameTmp.text = character.CharacterName;
            m_Image.sprite = character.Thumbnail;

            _fullDialogText = dialog;

            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            _typingCoroutine = StartCoroutine(TypeText(dialog));



            return this;
        }

        public UIVSDialogFrame ShowChoices(DialogChoiceData[] choices)
        {
            ClearChoices();
            m_ChoiceRoot.SetActive(true);

            foreach (var choiceData in choices)
            {
                var item = Spawn("Choices", m_ChoicePrefab, m_ChoiceContainer.transform);
                UIDialogChoiceItem choice = item.GetComponent<UIDialogChoiceItem>();
                if (choice != null)
                {
                    choice.Bind(choiceData.ChoiceID);
                }
            }

            return this;
        }

        public void ClearChoices()
        {
            DespawnAll("Choices");
            m_ChoiceRoot.SetActive(false);
        }

        private void Next()
        {
            _eventManager.Publish(new NextDialogEvent());
        }
        private void Auto()
        {
            _eventManager.Publish(new AutoPlayDialogEvent());
        }
        private void Skip()
        {
            _eventManager.Publish(new SkipDialogEvent());
        }

        private void DispatchChoiceSelection(SelectChoice selectChoice)
        {
            ClearChoices();
        }

        private IEnumerator TypeText(string dialog)
        {
            _isTyping = true;
            m_DialogTmp.text = string.Empty;

            for (int i = 0; i < dialog.Length; i++)
            {
                m_DialogTmp.text += dialog[i];
                yield return new WaitForSeconds(m_CharInterval);
            }

            FinishTyping();
        }

        private void FinishTyping()
        {
            _isTyping = false;
            m_DialogTmp.text = _fullDialogText;

            // Nếu đang auto -> tự next
            if (_autoMode)
            {
                _eventManager.Publish(new NextDialogEvent());
            }
        }
    }
}