namespace SEP490G69.Addons.Localization
{
	using SEP490G69.Addons.Localization.Enums;

	public class ChangeLanguageEvent : IEvent
	{
		public ELocalizeLanguageType LanguageType {  get; set; }
	}
}