using SEP490G69.Battle.Cards;

namespace SEP490G69.Battle.Combat
{
    public class DoomEffect : ISpecialStatusEffect
    {
        private StatusEffectSO _effectSO;

        private CustomVariable _varReceiveAmountPerTurn;
        private CustomVariable _varExplodeLimit;

        public void SetSO(StatusEffectSO effectSO)
        {
            _effectSO = effectSO;
            _varReceiveAmountPerTurn = _effectSO.GetVariableByName("received_doom_count");
            _varExplodeLimit = _effectSO.GetVariableByName("explode_limit");
        }

        public void OnTurnStart(BaseCombatActor self, BaseCombatActor opponent)
        {
            int receiveAmount = _varReceiveAmountPerTurn.GetValue<int>();

            for (int i = 0; i < receiveAmount; i++)
            {
                self.AddStatusEffectById(_effectSO.EffectId);
            }
        }

        public void OnTurnEnd(BaseCombatActor self, BaseCombatActor opponent)
        {
            RuntimeStatusEffect effect = self.EffectsManager.GetById(_effectSO.EffectId);
            if (effect != null)
            {
                int explodeLimit = _varExplodeLimit.GetValue<int>();
                if (effect.Stack >= explodeLimit)
                {
                    // Explode here.
                }
            }
        }

        #region Unused
        public void OnAfterAction(BaseCombatActor self, BaseCombatActor target) { }

        public void OnAfterBeingAttacked(float damage, BaseCombatActor self, BaseCombatActor attacker) { }

        public void OnApplied(BaseCombatActor self) { }

        public void OnBeforeAction(BaseCombatActor self, BaseCombatActor target) { }

        public void OnDiscarded(BaseCombatActor source) { }

        public void OnHitTarget(BaseCombatActor self, BaseCombatActor target) { }

        public void TriggerManually(BaseCombatActor self, BaseCombatActor attacker) { }

        #endregion
    }
}