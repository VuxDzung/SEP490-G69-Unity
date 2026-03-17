namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    /// <summary>
    /// Base attack card. Handle most of the attack card logic.
    /// </summary>
    public class BaseAttackCard : BaseCard
    {
        public BaseAttackCard(CardSO cardSO) : base(cardSO) { }

        //protected override void ExecuteAction(BaseBattleCharacterController source, BaseBattleCharacterController target)
        //{
        //    source.CalculateBaseDmg();

        //    float damage = source.GetCombatStatus(EStatusType.Damage).Value;

        //    Debug.Log($"Damage: {damage}");
        //    damage += CalculateExtraDmg(damage, source, target);

        //    source.StatOutputDmg.SetCurrentValue(damage);

        //    source.AnimationController.PlayAnimation("atk", (animName) => { });

        //    source.VFXController.PlayAtkVFX();

        //    target.AnimationController.PlayAnimation("take_dmg", (animName) => { });

        //    for (int i = 0; i < Data.AtkCount; i++)
        //    {
        //        target.ReceiveDamage(source.StatOutputDmg.Value, source);
        //    }

        //    OnAfterAttack(damage, source, target);
        //}

        protected override void ExecuteAction(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            source.CalculateBaseDmg();

            float damage = source.GetCombatStatus(EStatusType.Damage).Value;
            damage += CalculateExtraDmg(damage, source, target);

            source.StatOutputDmg.SetCurrentValue(damage);

            bool hasAttack = damage > 0;

            if (!hasAttack)
            {
                // No animation -> skip action -> trigger immediately.
                OnAnimationCompleted(source, target);
                return;
            }

            CombatCameraController.Singleton.ShakeCamera();
            CombatCameraController.Singleton.ZoomCamera(true);

            // Has animation -> create 2 barriers to wait for 2 animations.
            AnimationBarrier barrier = new AnimationBarrier(2, () =>
            {
                CombatCameraController.Singleton.ZoomCamera(false);
                OnAnimationCompleted(source, target);
            });

            // Attacker animation
            source.AnimationController.PlayAnimation("atk", (_) =>
            {
                barrier.Signal();
            });

            source.VFXController.PlayAtkVFX();

            // Target animation
            target.AnimationController.PlayAnimation("take_dmg", (_) =>
            {
                barrier.Signal();
            });

            // Damage apply
            for (int i = 0; i < Data.AtkCount; i++)
            {
                target.ReceiveDamage(source.StatOutputDmg.Value, source);
            }
        }

        protected virtual void OnAfterAttack(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {

        }

        protected virtual float CalculateExtraDmg(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            return 0;
        }

        protected override void OnAnimationCompleted(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            OnAfterAttack(source.StatOutputDmg.Value, source, target);
            base.OnAnimationCompleted(source, target);
        }
    }
}