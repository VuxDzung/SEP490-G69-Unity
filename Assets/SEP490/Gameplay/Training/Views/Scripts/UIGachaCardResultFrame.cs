using SEP490G69.Battle.Cards;

namespace SEP490G69.Training
{
    public class UIGachaCardResultFrame : GameUIFrame
    {
        private CardConfigSO _cardConfig;
        private CardConfigSO CardConfig => _cardConfig ??= ContextManager.Singleton.GetDataSO<CardConfigSO>();

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
        }
    }
}