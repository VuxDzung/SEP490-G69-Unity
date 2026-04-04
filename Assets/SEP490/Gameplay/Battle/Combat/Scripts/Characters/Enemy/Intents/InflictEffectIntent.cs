namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Addons.Localization;
    using SEP490G69.Battle.Cards;
    using System;
    using UnityEngine;

    public class InflictEffectIntent : BaseEnemyIntent
    {
        public override EIntentAction IntentType => EIntentAction.InflictEffect;

        public override BaseCombatActor SelectTarget()
        {
            return _battleManager.Player;
        }

        public override void Execute(Action onCompleted)
        {
            var target = SelectTarget();

            for (int i = 0; i < _data.InflictAmount; i++)
            {
                _battleManager.Player.AddStatusEffectById(_data.InflictEffectId);
            }

            _owner.ExecuteVfxs(_data.VfxList, _battleManager.Player, (opponent) =>
            {
                onCompleted?.Invoke();
            });
        }

        public override void Preview()
        {
            StatusEffectSO effectSO = _owner.EffectsConfig.GetById(_data.InflictEffectId);

            if (effectSO == null)
            {
                Debug.LogError($"[InflictEffectIntent.Preview fatal error] Effect SO with id {_data.InflictEffectId} is not configured!");
                return;
            }

            string localizedName = ContextManager.Singleton
                                                 .ResolveGameContext<LocalizationManager>()
                                                 .GetText(GameConstants.LOCALIZE_CATEGORY_STATUS_EFFECT_NAMES, effectSO.EffectName);

            _owner.IntentUIUpdater.MakeIntent(localizedName, UnityEngine.Color.yellow, null);
        }
    }
}