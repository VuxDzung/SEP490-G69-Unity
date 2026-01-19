namespace SEP490G69
{
    using UnityEngine;

    public class FadeInHandler : NarrativeActionHandlerBase
    {
        public FadeInHandler(ContextManager contextManager) : base(contextManager)
        {
        }

        public override string ActionId => GameConstants.ACTION_FADE_IN;

        public override void Execute(DialogEvent ev)
        {
            var fadeTime = ev.Parameters
                .GetParameter(GameConstants.PARAM_FADE_TIME)?
                .GetFloatValue() ?? 0f;

            FadingController.Singleton.FadeIn(fadeTime);
        }
    }
}