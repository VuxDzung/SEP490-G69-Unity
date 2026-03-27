namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CombatTurnProcessor : IDisposable
    {
        public event Action onPlayerEndTurn;
        public event Action onEnemyEndTurn;

        private PlayerBattleCharaterController _player;
        private EnemyCombatController _enemy;

        private CardConfigSO _cardConfig;

        public void Initialize(PlayerBattleCharaterController player, EnemyCombatController enemy)
        {
            _player = player;
            _enemy = enemy;
            _cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();

            _player.onFlowEventChanged += _player_onFlowEventChanged;
            _enemy.onFlowEventChanged += _enemy_onFlowEventChanged;
        }

        public void Dispose()
        {
            _player.onFlowEventChanged -= _player_onFlowEventChanged;
            _enemy.onFlowEventChanged -= _enemy_onFlowEventChanged;
        }

        private void _enemy_onFlowEventChanged(ETurnFlowEvent arg1, BaseBattleCharacterController arg2)
        {
            if (arg1 == ETurnFlowEvent.AfterResetActionGaugue)
            {
                onPlayerEndTurn?.Invoke();
                _enemy.UnpauseBar();
                _player.UnpauseBar();
            }
        }

        private void _player_onFlowEventChanged(ETurnFlowEvent ev, BaseBattleCharacterController owner)
        {
            if (ev == ETurnFlowEvent.AfterResetActionGaugue)
            {
                onEnemyEndTurn?.Invoke();
                _enemy.UnpauseBar();
                _player.UnpauseBar();
            }
        }

        public void Update(float dt)
        {
            _player.UpdateActionGauge(dt);
            _enemy.UpdateActionGauge(dt);
        }

        public void PlayerTurn()
        {
            _enemy.PauseBar();

            _player.StartTurn();
            _player.DrawThreeCards(out IReadOnlyList<CardSO> cards);

            GameUIManager.Singleton
                .GetFrame(GameConstants.FRAME_ID_COMBAT)
                .AsFrame<UICombatFrame>()
                .DisplayDrawnCards(cards);
        }

        public void EnemyTurn()
        {
            _player.PauseBar();
            _enemy.PauseBar();

            _enemy.DetermineCards(_player, (selectedCardId) =>
            {
                if (selectedCardId.Equals("REST"))
                {
                    Debug.Log("Enemy choose rest");
                }
                else
                {
                    CardSO card = _cardConfig.GetCardById(selectedCardId);

                    if (card == null)
                    {
                        return;
                    }

                    GameUIManager.Singleton
                                 .GetFrame(GameConstants.FRAME_ID_COMBAT)
                                 .AsFrame<UICombatFrame>()
                                 .SpawnEnemyCard(card);
                }
            });
        }

        public void ExecutePlayerCard()
        {
            _player.ExecuteCard(_enemy);

            GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                       .AsFrame<UICombatFrame>().ClearAllCards();
        }
    }
}