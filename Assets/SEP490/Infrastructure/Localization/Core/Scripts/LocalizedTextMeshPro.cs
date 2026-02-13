namespace SEP490G69.Addons.Localization
{
    using TMPro;
    using UnityEngine;

    public class LocalizedTextMeshPro : MonoBehaviour
    {
        [SerializeField] private string category;
        [SerializeField] private string elementId;
        [SerializeField] private TextMeshProUGUI tmp;

        private EventManager _eventManager;
        private LocalizationManager _localizationManager;

        private void OnEnable()
        {
        }

        private void Start()
        {
            if (tmp == null)
                tmp = GetComponent<TextMeshProUGUI>();
            if (_eventManager == null)
                _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
            if (_localizationManager == null)
                _localizationManager = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();

            _eventManager.Subscribe<ChangeLanguageEvent>(OnLanguageChanged);

            string text = _localizationManager.GetText(category, elementId);
            if (text != null)
            {
                tmp.text = text;
            }
        }
        private void OnDestroy()
        {
            _eventManager.Unsubscribe<ChangeLanguageEvent>(OnLanguageChanged);
        }

        private void OnLanguageChanged(ChangeLanguageEvent ev)
        {
            string text = _localizationManager.GetText(category, elementId, ev.LanguageType);
            if (text != null)
            {
                tmp.text = text;
            }
        }
    }
}