namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using UnityEngine;

    public class CombatTurnSystem
    {
        private PlayerBattleCharaterController _player;
        private EnemyCombatController _enemy;

        private CardConfigSO _cardConfig;

        public void Initialize(PlayerBattleCharaterController player, EnemyCombatController enemy)
        {
            _player = player;
            _enemy = enemy;
            _cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();

            _player.OnActionFinished = () =>
            {
                _player.EndCurrentTurn();

                _enemy.UnpauseBar();
                _player.UnpauseBar();
            };

            _enemy.OnActionFinished = () =>
            {
                _enemy.EndCurrentTurn();
                _enemy.UnpauseBar();
                _player.UnpauseBar();
            };
        }

        public void Update(float dt)
        {
            _player.UpdateCharge(dt);
            _enemy.UpdateCharge(dt);
        }

        public void PlayerTurn()
        {
            _enemy.PauseBar();

            _player.OnTurnStart();
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
            if (_player.SelectedCard == null)
            {
                Debug.Log("<color=yellow>[CombatTurnSystem]</color> No card selected. Skip this action");
            }
            else
            {
                _player.ExecuteCard(_player, _enemy);
            }

            GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                       .AsFrame<UICombatFrame>().ClearAllCards();
        }
    }
}