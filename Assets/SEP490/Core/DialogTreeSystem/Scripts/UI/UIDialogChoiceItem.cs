namespace SEP490G69
{
    using SEP490G69.Addons.Localization;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SelectChoice : IEvent
    {
        public string ChoiceID { get; set; }
    }

    public class UIDialogChoiceItem : MonoBehaviour, IPooledObject
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Button m_Button;

        private LocalizationManager _localizationManager;
        private EventManager _eventManager;
        private string choiceID;

        public void Spawn()
        {
            if (_eventManager == null) _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
            if (_localizationManager == null) _localizationManager = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();

            m_Button.onClick.AddListener(OnClick);
        }
        public void Despawn()
        {
            m_Button.onClick.RemoveListener(OnClick);
        }

        public void Bind(string choiceID)
        {
            this.choiceID = choiceID;

            m_Text.text = _localizationManager.GetText(GameConstants.LOCALIZE_CATEGORY_DIALOG_CHOICE, choiceID);
        }

        private void OnClick()
        {
            _eventManager.Publish(new SelectChoice { ChoiceID = choiceID });
        }
    }
}