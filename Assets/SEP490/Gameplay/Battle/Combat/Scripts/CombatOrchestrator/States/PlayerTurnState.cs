namespace SEP490G69.Combat.Battle
{
    using SEP490G69.Battle.Combat;
    using System;
    using UnityEngine;

    public class PlayerTurnState : ITurnState, IDisposable
    {
        private readonly SceneCombatController _sceneController;
        private PlayerActorController _source;
        private EnemyActorController _opponent;

        private readonly Timer _delayEndTurnTimer = new Timer();

        public PlayerTurnState(SceneCombatController sceneController, PlayerActorController playerActor, EnemyActorController opponent)
        {
            _sceneController = sceneController;
            _source = playerActor;
            _opponent = opponent;
            TimerManager.AddTimer(_delayEndTurnTimer);
        }

        public void Dispose()
        {
            TimerManager.RemoveTimer(_delayEndTurnTimer);
        }

        public void OnTurnStarted()
        {
            Debug.Log("<color=green>[PlayerTurnState.OnTurnStarted]</color> Start turn");
            if (_source.IsSkippingTurn == true)
            {
                Debug.Log("<color=green>[PlayerTurnState.OnTurnStarted]</color> Cannot enter current turn because you're stunned!");
                _delayEndTurnTimer.StartTimer(0.5f);
                _delayEndTurnTimer.OnExpired = (timer) =>
                {
                    _sceneController.TurnProcessor.ChangeToEnemyTurn();
                };
                _source.StartTurn();
            }
            else
            {
                _source.StartTurn();
                _source.DrawCards(out var cards);
                _sceneController.CombatUI.DisplayPlayerCards(cards, _source.CardsService, _source.StatsManager.GetValue(EStatusType.Stamina));
            }
        }

        public void OnTurnCompleted()
        {
            Debug.Log("<color=green>[PlayerTurnState.OnTurnStarted]</color> End turn");
            _sceneController.CombatUI.ClearAllUICards();
            _source.EndCurrentTurn();
        }
    }
}