using SEP490G69.Addons.Localization;
using SEP490G69.Battle.Cards;
using System;

namespace SEP490G69.Battle.Combat
{
    public class ShieldIntent : BaseEnemyIntent
    {
        public override EIntentAction IntentType => EIntentAction.Shield;

        public override BaseCombatActor SelectTarget()
        {
            return _owner;
        }

        public override void Execute(Action onCompleted)
        {
            _owner.StackShield(_data.BaseDefend, _data.DefendMultiplier);
            _owner.ExecuteVfxs(_data.VfxList, _battleManager.Player, (opponent) =>
            {
                onCompleted?.Invoke();
            });
        }

        public override void Preview()
        {
            StatusEffectSO effectSO = _owner.EffectsConfig.GetById(_data.GainEffectId);

            float extraShield = _owner.CalculateReceivedShield(_data.BaseDefend, _data.DefendMultiplier);

            _owner.IntentUIUpdater.MakeIntent(extraShield.ToString(), UnityEngine.Color.blue, null);
        }
    }
}