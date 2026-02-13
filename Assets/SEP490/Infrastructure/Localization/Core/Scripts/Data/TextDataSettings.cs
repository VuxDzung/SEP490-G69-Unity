namespace SEP490G69.Addons.Localization
{
	using SEP490G69.Addons.Localization.Enums;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[System.Serializable]
	public class TextDataSettings 
	{
		[SerializeField] private string id;
		[SerializeField] private List<LocalizeTextData> textList = new List<LocalizeTextData>();

		public string Id => id;
		public List<LocalizeTextData> TextList => textList;

		public string GetText(ELocalizeLanguageType languageType)
		{
			LocalizeTextData textData = TextList.FirstOrDefault(t => t.Language == languageType);
			if (textData == null)
			{
				return string.Empty;
			}
			return textData.Text;
		}
	}
}