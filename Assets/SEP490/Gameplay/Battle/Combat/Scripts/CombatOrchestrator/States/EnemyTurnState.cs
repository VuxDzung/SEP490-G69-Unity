namespace SEP490G69.Combat.Battle
{
    using System;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class EnemyTurnState : ITurnState, IDisposable
    {
        private SceneCombatController _sceneController;
        private EnemyActorController _source;
        private PlayerActorController _opponent;

        private readonly Timer _delayPerformIntentTimer = new Timer();
        private readonly Timer _delaySwitchTurnTimer = new Timer();

        public EnemyTurnState(SceneCombatController sceneController, EnemyActorController source, PlayerActorController opponent)
        {
            _sceneController = sceneController;
            _source = source;
            _opponent = opponent;

            TimerManager.AddTimer(_delayPerformIntentTimer);
            TimerManager.AddTimer(_delaySwitchTurnTimer);
        }

        public void Dispose()
        {
            TimerManager.RemoveTimer(_delayPerformIntentTimer);
            TimerManager.RemoveTimer(_delaySwitchTurnTimer);
        }

        public void OnTurnStarted()
        {
            Debug.Log("<color=green>[EnemyTurnState.OnTurnStarted]</color> Start turn");

            if (_source.IsSkippingTurn == true)
            {
                Debug.Log("<color=green>[PlayerTurnState.OnTurnStarted]</color> Cannot enter current turn because you're stunned!");
                _delaySwitchTurnTimer.OnExpired = (timer) =>
                {
                    _sceneController.ChangeToPlayerTurn();
                };
                _delaySwitchTurnTimer.StartTimer(2f);
                _source.StartTurn();
            }
            else
            {
                _source.StartTurn();
                _delayPerformIntentTimer.OnExpired = (timer) =>
                {
                    _source.ExecuteTurn();
                };
                _delayPerformIntentTimer.StartTimer(UnityEngine.Random.Range(0.75f, 1.25f));
            }
        }

        public void OnTurnCompleted()
        {
            Debug.Log("<color=green>[EnemyTurnState.OnTurnCompleted]</color> End turn");
            _source.EndTurn();
        }
    }
}