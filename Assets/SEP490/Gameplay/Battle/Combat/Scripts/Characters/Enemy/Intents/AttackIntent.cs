using System;

namespace SEP490G69.Battle.Combat
{
    public class AttackIntent : BaseEnemyIntent
    {
        public override EIntentAction IntentType => EIntentAction.Attack;

        public override BaseCombatActor SelectTarget()
        {
            return _battleManager.Player;
        }

        public override void Execute(Action onCompleted)
        {
            var opponent = SelectTarget();

            float damage = CalculateDamage(_data.BaseDamage, _data.AttackMultiplier);

            _owner.PlayAtkSfx();

            CombatCameraController.Singleton.ZoomCamera(true);
            CombatCameraController.Singleton.ShakeCamera();

            // Animation
            AnimationBarrier barrier = new AnimationBarrier(2, () =>
            {
                OnAnimationCompleted(opponent, (opponent) =>
                {
                    onCompleted?.Invoke();
                });
                CombatCameraController.Singleton.ZoomCamera(false);
            });

            _owner.AnimationController.PlayAnimation("atk", (_) =>
            {
                barrier.Signal();
            });

            _owner.VFXController.PlayVFXById("vfx_atk");

            if (opponent.CanEvade(_owner) == true)
            {
                opponent.AnimationController.PlayAnimation("dodge", (_) =>
                {
                    barrier.Signal();
                });
            }
            else
            {
                opponent.AnimationController.PlayAnimation("take_dmg", (_) =>
                {
                    barrier.Signal();
                });

                opponent.ReceiveAttack(damage, _owner);
            }
        }

        public override void Preview()
        {
            float damage = CalculateDamage(_data.BaseDamage, _data.AttackMultiplier);
            _owner.IntentUIUpdater.MakeIntent(damage.ToString(), UnityEngine.Color.red, null);
        }

        private float CalculateDamage(float damage, float multiplier)
        {
            return (float)(damage + _owner.StatsManager.GetValue(EStatusType.Attack) * multiplier);
        }

        private void OnAnimationCompleted(BaseCombatActor opponent, Action<BaseCombatActor> onCompleted)
        {
            _owner.ExecuteVfxs(_data.VfxList, opponent, onCompleted);
        }
    }
}