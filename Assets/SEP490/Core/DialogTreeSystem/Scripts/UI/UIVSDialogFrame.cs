namespace SEP490G69
{
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
        [SerializeField] private Transform m_ChoicePrefab;

        private LocalizationManager _localization;
        private CharacterConfigSO _characterConfig;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (_localization == null) _localization = ContextManager.Singleton.GetGameContext<LocalizationManager>();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
        }

        public UIVSDialogFrame RenderDialog(string speakerID, string dialogID)
        {
            string dialog = _localization.GetText(GameConstants.LOCALIZE_CATEGORY_DIALOG, dialogID);
            string characterName = _characterConfig.GetCharacter(speakerID).CharacterName;
            m_Image.sprite = _characterConfig.GetCharacter(speakerID).Avatar;

            return this;
        }

        public UIVSDialogFrame ShowChoices(DialogChoiceData[] choices)
        {
            ClearChoices();
            m_ChoiceRoot.SetActive(true);

            foreach (var choiceData in choices)
            {
                var item = Spawn("Choices", m_ChoicePrefab, m_ChoiceRoot.transform);
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
    }
}