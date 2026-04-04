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

        protected override void ExecuteAction(PlayerActorController source, BaseCombatActor target)
        {
            source.CalculateSelectedCardDmg(true);

            bool hasCrit = source.HasCrit(CheckForceCritCondition(source));
            float critMul = source.CaculateCritMul();
            critMul = (float)System.Math.Round(critMul, 1);

            float damage = source.StatsManager.GetValue(EStatusType.Damage) * (hasCrit ? critMul : 1f);

            Debug.Log($"Raw damage of {source.CharacterSO.CharacterName}: {damage}");

            damage += CalculateExtraDmg(damage, source, target);

            Debug.Log($"Damage after receive extra dmg of {source.CharacterSO.CharacterName}: {damage}");

            source.StatsManager.SetCurrentValue(EStatusType.Damage, damage);

            Debug.Log($"Final Damage of {source.CharacterSO.CharacterName}: {damage}");

            bool hasAttack = damage > 0;

            if (!hasAttack)
            {
                OnAnimationCompleted(source, target);
                return;
            }

            CombatCameraController.Singleton.ShakeCamera();
            CombatCameraController.Singleton.ZoomCamera(true);

            source.PlayAtkSfx();

            source.VFXController.PlayAtkVFX(0.15f);

            if (hasCrit == true) source.SpawnCritToast(critMul);

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

            source.VFXController.PlayVFXById("vfx_atk");

            if (target.CanEvade(source))
            {
                target.SpawnDodgeToast();
                target.AnimationController.PlayAnimation("dodge", (_) =>
                {
                    barrier.Signal();
                });
            }
            else
            {
                // Target animation
                target.AnimationController.PlayAnimation("take_dmg", (_) =>
                {
                    barrier.Signal();
                });

                // Damage apply
                for (int i = 0; i < Data.AtkCount; i++)
                {
                    target.ReceiveAttack(source.StatsManager.GetValue(EStatusType.Damage), source);
                }
            }
        }

        protected virtual void OnAfterAttack(float curDmg, BaseCombatActor source, BaseCombatActor target) { }

        protected override void OnAnimationCompleted(PlayerActorController source, BaseCombatActor target)
        {
            OnAfterAttack(source.StatsManager.GetValue(EStatusType.Damage), source, target);
            base.OnAnimationCompleted(source, target);
        }

        /// <summary>
        /// This method ensure to have 100% crit chance in a specific condition.
        /// By default, there's no 100% crit.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckForceCritCondition(BaseCombatActor source)
        {
            return false;
        }
    }
}