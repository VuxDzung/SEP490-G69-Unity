namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Combat.Battle;
    using System;

    public interface ITurnState
    {
        public void OnTurnStarted();
        public void OnTurnCompleted();
    }

    public class CombatTurnProcessor : IDisposable
    {
        private PlayerActorController _player;
        private EnemyActorController _enemy;

        private CardConfigSO _cardConfig;

        private ITurnState _currentTurnState;

        private PlayerTurnState _playerTurnState;
        private EnemyTurnState _enemyTurnState;

        public void Initialize(SceneCombatController sceneController, PlayerActorController player, EnemyActorController enemy)
        {
            _player = player;
            _enemy = enemy;
            _cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();

            _playerTurnState = new PlayerTurnState(sceneController, player, _enemy);
            _enemyTurnState = new EnemyTurnState(sceneController, _enemy, _player);
        }

        public void Dispose()
        {
            if (_playerTurnState != null) _playerTurnState.Dispose();
            if (_enemyTurnState != null) _enemyTurnState.Dispose();
        }

        public void ChangeTurn(ITurnState newState)
        {
            if (_currentTurnState != null)
            {
                _currentTurnState.OnTurnCompleted();
            }
            _currentTurnState = newState;
            _currentTurnState.OnTurnStarted();
        }

        public void ChangeToPlayerTurn()
        {
            ChangeTurn(_playerTurnState);
        }

        public void ChangeToEnemyTurn()
        {
            ChangeTurn(_enemyTurnState);
        }

        public void ExecutePlayerCard()
        {
            _player.ExecuteCard(_enemy);
            //GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
            //           .AsFrame<UICombatFrame>().ClearAllCards();
        }
    }
}