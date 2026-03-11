namespace SEP490G69.Battle.Combat
{
    using System;

    public class BattleStateMachine
    {
        public event Action<EBattleState> OnStateChanged;

        public EBattleState CurrentState { get; private set; }

        public void ChangeState(EBattleState state)
        {
            if (CurrentState == state)
                return;

            CurrentState = state;
            OnStateChanged?.Invoke(state);
        }
    }
}