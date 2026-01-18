namespace SEP490G69.Addons.Localization
{
    using TMPro;
    using UnityEngine;

    public class LocalizedTextMeshPro : MonoBehaviour
    {
        [SerializeField] private string category;
        [SerializeField] private string elementId;
        [SerializeField] private TextMeshProUGUI tmp;

        private EventManager eventManager;
        private LocalizationManager localizationManager;

        private void Awake()
        {
            if (tmp == null) tmp = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            eventManager.Subscribe<ChangeLanguageEvent>(OnLanguageChanged);
        }
        private void OnDisable()
        {
            eventManager.Unsubscribe<ChangeLanguageEvent>(OnLanguageChanged);
        }

        private void OnLanguageChanged(ChangeLanguageEvent ev)
        {
            string text = localizationManager.GetText(category, elementId, ev.LanguageType);
            if (text != null)
            {
                tmp.text = text;
            }
        }
    }
}