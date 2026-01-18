namespace SEP490G69.Addons.Localization
{
	using SEP490G69.Addons.Localization.Enums;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[CreateAssetMenu(fileName = "TextConfig", menuName = OrganizationConstants.NAMESPACE + "/Addons/Localization/Text Config")]
	public class TextConfigSO : ScriptableObject
	{
		[SerializeField] private string category;
		[SerializeField] private List<TextDataSettings> textDataList;

		public string Category => category;
		public List<TextDataSettings> TextDataList => textDataList;

		public string GetText(string elementId, ELocalizeLanguageType language)
		{
			TextDataSettings textSettings = textDataList.FirstOrDefault(t => t.Id.Equals(elementId));
			if (textSettings == null)
			{
				return string.Empty;
			}
			return textSettings.GetText(language);
		}
	}
}