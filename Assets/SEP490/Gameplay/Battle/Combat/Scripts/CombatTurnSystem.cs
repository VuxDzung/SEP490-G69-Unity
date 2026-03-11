namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using UnityEngine;

    public class CombatTurnSystem
    {
        private PlayerBattleCharaterController _player;
        private EnemyCombatController _enemy;

        public void Initialize(PlayerBattleCharaterController player, EnemyCombatController enemy)
        {
            _player = player;
            _enemy = enemy;
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
            _enemy.DetermineCards(_player);
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

            _player.EndCurrentTurn();

            _enemy.UnpauseBar();
            _player.UnpauseBar();

            GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                                   .AsFrame<UICombatFrame>().ClearAllCards();
        }
    }
}