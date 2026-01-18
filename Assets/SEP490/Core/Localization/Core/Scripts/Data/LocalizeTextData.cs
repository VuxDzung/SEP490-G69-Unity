namespace SEP490G69.Addons.Localization
{
	using SEP490G69.Addons.Localization.Enums;
	using UnityEngine;

	[System.Serializable]
	public class LocalizeTextData 
	{
		[SerializeField] private ELocalizeLanguageType language;
		[SerializeField, TextArea] private string text;

		public ELocalizeLanguageType Language => language;
		public string Text => text;
	}
}