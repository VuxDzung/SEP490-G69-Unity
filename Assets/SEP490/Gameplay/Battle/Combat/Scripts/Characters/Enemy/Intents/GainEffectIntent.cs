using SEP490G69.Addons.Localization;
using SEP490G69.Battle.Cards;
using System;

namespace SEP490G69.Battle.Combat
{
    public class GainEffectIntent : BaseEnemyIntent
    {
        public override EIntentAction IntentType => EIntentAction.GainEffect;

        public override BaseCombatActor SelectTarget()
        {
            return _owner;
        }

        public override void Execute(Action onCompleted)
        {
            for (int i = 0; i < _data.GainAmount; i++)
            {
                _owner.AddStatusEffectById(_data.GainEffectId);
            }

            _owner.ExecuteVfxs(_data.VfxList, _battleManager.Player, (opponent) =>
            {
                onCompleted?.Invoke();
            });
        }

        public override void Preview()
        {
            StatusEffectSO effectSO = _owner.EffectsConfig.GetById(_data.GainEffectId);
            string localizedName = ContextManager.Singleton
                                                 .ResolveGameContext<LocalizationManager>()
                                                 .GetText(GameConstants.LOCALIZE_CATEGORY_STATUS_EFFECT_NAMES, effectSO.EffectName);

            _owner.IntentUIUpdater.MakeIntent(localizedName, UnityEngine.Color.green, null);
        }
    }
}