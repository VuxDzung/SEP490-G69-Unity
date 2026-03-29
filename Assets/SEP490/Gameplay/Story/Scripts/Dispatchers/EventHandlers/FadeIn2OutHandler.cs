namespace SEP490G69
{
    using SEP490G69.Addons.LoadScreenSystem;
    using UnityEngine;

    public class FadeIn2OutHandler : NarrativeActionHandlerBase
    {
        private readonly NarrativeManager _dialogManager;

        public FadeIn2OutHandler(ContextManager contextManager) : base(contextManager)
        {
            _dialogManager = contextManager.ResolveGameContext<NarrativeManager>();    
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

            string onCompleteAction = onCompleted.GetStringValue();
            switch (onCompleteAction)
            {
                case "LoadScene":
                    string sceneName = parameters.GetParameter("sceneName").GetStringValue();
                    SceneLoader.Singleton.StartLoadScene(sceneName);
                    break;
                case "StartDialogTree":
                    var treeId = parameters.GetParameter("dialogTreeId");
                    var dialogId = parameters.GetParameter("dialogId");
                    if (treeId != null)
                    {
                        _dialogManager.StartTree(treeId.GetStringValue(), dialogId != null ? dialogId.GetStringValue() : "");
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnFadeOutCompleted(ParameterInspectorData[] parameters)
        {
            // Extend later.
        }
    }
}