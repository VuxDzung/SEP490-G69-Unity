namespace SEP490G69.Addons.Localization
{
	using SEP490G69.Addons.Localization.Enums;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

	public class LocalizationManager : MonoBehaviour, IGameContext
	{
		public const string LOCALIZED_TEXT_LIST_ID = "{0}.{1}";

		[SerializeField] private LocalizedConfigSO config;

		private Dictionary<string, TextConfigSO> textConfigLookUp = new Dictionary<string, TextConfigSO>();
		private Dictionary<string, TextDataSettings> localizedTextLookUp = new Dictionary<string, TextDataSettings>();

		[SerializeField] private ELocalizeLanguageType currentLanguage;

		private EventManager eventManager;

		private EventManager EventManager
		{
			get
			{
				if (eventManager == null)
				{
					eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
				}
				return eventManager;
			}
		}

		public ELocalizeLanguageType CurrentLanguage => currentLanguage;

        public void SetManager(ContextManager manager)
        {
            
        }

        private void Awake()
		{
			if (config == null)
			{
				config = Resources.Load<LocalizedConfigSO>("Localization/LocalizedConfig");
            }

			textConfigLookUp.Clear();
			localizedTextLookUp.Clear();
			foreach (var textConfig in config.ConfigList)
			{
				textConfigLookUp.Add(textConfig.Category, textConfig);
				foreach (var textData in textConfig.TextDataList)
				{
					string id = string.Format(LOCALIZED_TEXT_LIST_ID, textConfig.Category, textData.Id);
					localizedTextLookUp.Add(id, textData);
				}
			}

			LoadExistedLanguage();
        }

		public void SetLanguage(ELocalizeLanguageType language)
		{
			currentLanguage = language;
            EventManager.Publish(new ChangeLanguageEvent
			{
				LanguageType = currentLanguage
			});
		}

		public string GetText(string category, string textId)
		{
			return GetText(category, textId, CurrentLanguage);
		}

		public string GetText(string category, string elementId, ELocalizeLanguageType languageType)
		{
			string id = string.Format(LOCALIZED_TEXT_LIST_ID, category, elementId);
			if (localizedTextLookUp.ContainsKey(id))
			{
				TextDataSettings textSettings = localizedTextLookUp[id];
				if (textSettings != null)
				{
					return textSettings.GetText(languageType);
				}
			}
			return string.Empty;
		}

		public TextConfigSO GetTextConfig(string category)
		{
			if (localizedTextLookUp.ContainsKey(category))
			{
				return textConfigLookUp[category];
			}
			return null;
		}

        private void LoadExistedLanguage()
        {
            string langStr = PlayerPrefs.GetString("Language");
            if (!string.IsNullOrEmpty(langStr))
            {
                ELocalizeLanguageType lang = Enum.Parse<ELocalizeLanguageType>(langStr);
                SetLanguage(lang);
            }
            else
            {
                Debug.Log("No selected lang yet");
            }
        }
    }
}