namespace SEP490G69
{
    using UnityEngine;

    public class FadeIn2OutHandler : NarrativeActionHandlerBase
    {
        private readonly GameDialogManager _dialogManager;

        public FadeIn2OutHandler(ContextManager contextManager) : base(contextManager)
        {
            _dialogManager = contextManager.ResolveGameContext<GameDialogManager>();    
        }

        public override string ActionId => GameConstants.ACTION_FADE_IN_2_OUT;

        public override void Execute(DialogEvent ev)
        {
            var parameters = ev.Parameters;

            var fadeTime = parameters
                .GetParameter(GameConstants.PARAM_FADE_TIME)?
                .GetFloatValue() ?? 0f;

            var inFadeTime = parameters
                .GetParameter(GameConstants.PARAM_IN_FADE_TIME)?
                .GetFloatValue() ?? 0f;

            FadingController.Singleton.FadeIn2Out(
                fadeTime,
                inFadeTime,
                () => OnFadeInCompleted(parameters),
                () => OnFadeOutCompleted(parameters)
            );
        }

        private void OnFadeInCompleted(ParameterInspectorData[] parameters)
        {
            var onCompleted = parameters.GetParameter("onFadeInCompleted");
            if (onCompleted == null) return;

            if (onCompleted.GetStringValue() == "StartDialogTree")
            {
                var treeId = parameters.GetParameter("dialogTreeId");
                if (treeId != null)
                {
                    _dialogManager.StartTree(treeId.GetStringValue());
                }
            }
        }

        private void OnFadeOutCompleted(ParameterInspectorData[] parameters)
        {
            // Extend later.
        }
    }
}