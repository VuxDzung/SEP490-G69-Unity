namespace SEP490G69
{
    using SEP490G69.PlayerProfile;
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIDialogFrame : GameUIFrame
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

        private CharacterConfigSO _characterConfig;

        private Coroutine _typingCoroutine;
        private string _fullDialogText;

        private bool _autoMode;

        private PlayerProfileController _profileController;
        private PlayerProfileController ProfileController
        {
            get
            {
                if (_profileController == null)
                {
                    _profileController = ContextManager.Singleton.ResolveGameContext<PlayerProfileController>();
                }
                return _profileController;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (_characterConfig == null) _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
            EventManager.Subscribe<SelectChoice>(DispatchChoiceSelection);
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
            EventManager.Unsubscribe<SelectChoice>(DispatchChoiceSelection);
            ClearChoices();
        }

        public UIDialogFrame RenderDialog(string speakerID, string dialogID)
        {
            string dialog = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_DIALOG, dialogID);

            //if (bgSprite != null)
            //{
            //    m_BgImage.sprite = bgSprite;
            //    m_BgImage.enabled = true;
            //}
            //else
            //{
            //    m_BgImage.sprite = null;
            //    m_BgImage.enabled = false;
            //}

            string playerName = ProfileController.GetPlayerName(PlayerPrefs.GetString(GameConstants.PREF_KEY_PLAYER_ID));

            if (dialog.Contains("ch_0004"))
            {
                dialog = dialog.Replace("ch_0004", playerName);
            }

            BaseCharacterSO character = _characterConfig.GetCharacterById(speakerID);
            if (character != null)
            {
                m_Image.enabled = true;
                if (speakerID == "ch_0004")
                {
                    m_SpeakerNameTmp.text = playerName;
                }
                else
                {
                    m_SpeakerNameTmp.text = character.CharacterName;
                }
                m_Image.sprite = character.FullBodyImg;
            }
            else
            {
                m_Image.enabled = false;
                m_SpeakerNameTmp.text = string.Empty;
            }

            _fullDialogText = dialog;

            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }

            _typingCoroutine = StartCoroutine(TypeText(dialog));

            return this;
        }

        public UIDialogFrame ShowChoices(DialogChoiceData[] choices)
        {
            ClearChoices();
            m_ChoiceRoot.SetActive(true);

            foreach (var choiceData in choices)
            {
                var item = Spawn(GameConstants.POOL_UI_DIALOG_CHOICES, m_ChoicePrefab, m_ChoiceContainer.transform);
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
            DespawnAll(GameConstants.POOL_UI_DIALOG_CHOICES);
            m_ChoiceRoot.SetActive(false);
        }

        private void Next()
        {
            EventManager.Publish(new NextDialogEvent());
        }
        private void Auto()
        {
            EventManager.Publish(new AutoPlayDialogEvent());
        }
        private void Skip()
        {
            EventManager.Publish(new SkipDialogEvent());
        }

        private void DispatchChoiceSelection(SelectChoice selectChoice)
        {
            ClearChoices();
        }

        private IEnumerator TypeText(string dialog)
        {
            if (string.IsNullOrEmpty(dialog))
            {
                FinishTyping();
            }
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
            m_DialogTmp.text = _fullDialogText;

            // Nếu đang auto -> tự next
            if (_autoMode)
            {
                EventManager.Publish(new NextDialogEvent());
            }
        }
    }
}