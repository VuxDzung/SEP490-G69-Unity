namespace SEP490G69.Combat.Battle
{
    using System;
    using System.Collections;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class EnemyTurnState : ITurnState, IDisposable
    {
        private SceneCombatController _sceneController;
        private EnemyActorController _source;
        private PlayerActorController _opponent;

        private readonly Timer _delaySwitchTurnTimer = new Timer();

        public EnemyTurnState(SceneCombatController sceneController, EnemyActorController source, PlayerActorController opponent)
        {
            _sceneController = sceneController;
            _source = source;
            _opponent = opponent;
            TimerManager.AddTimer(_delaySwitchTurnTimer);
        }

        public void Dispose()
        {
            TimerManager.RemoveTimer(_delaySwitchTurnTimer);
        }

        public void OnTurnStarted()
        {
            Debug.Log("<color=green>[EnemyTurnState.OnTurnStarted]</color> Start turn");
            _delaySwitchTurnTimer.OnExpired = (timer) =>
            {
                _sceneController.TurnProcessor.ChangeToPlayerTurn();
            };
            _delaySwitchTurnTimer.StartTimer(2f);
        }

        public void OnTurnCompleted()
        {
            Debug.Log("<color=green>[EnemyTurnState.OnTurnCompleted]</color> End turn");
        }
    }
}