namespace SEP490G69.Addons.Localization
{
	using SEP490G69.Addons.Localization.Enums;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "LocalizedConfig", menuName = OrganizationConstants.NAMESPACE + "/Addons/Localization/Localization Config")]
	public class LocalizedConfigSO : ScriptableObject
	{
		[SerializeField] private List<TextConfigSO> configList = new List<TextConfigSO>();

		public List<TextConfigSO> ConfigList => configList;

		public string GetText(string category, string elementId, ELocalizeLanguageType languageType)
		{
			TextConfigSO config = configList.FirstOrDefault(c => c.Category.Equals(category));
			if (config == null)
			{
				return string.Empty;
			}
			return config.GetText(elementId, languageType);
		}
	}
}