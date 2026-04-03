namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Shared;
    using System.Collections.Generic;

    public class CombatUIUpdater
    {
        public void UpdateEnergy(PlayerActorController player, EnemyActorController enemy)
        {
            UICombatFrame frame = GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                                                         .AsFrame<UICombatFrame>();
        }

        public void UpdateStats(PlayerActorController player, EnemyActorController enemy)
        {
            UICombatFrame frame = GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                                               .AsFrame<UICombatFrame>();

            frame.SetPlayerCharHP(player.StatsManager.GetValue(EStatusType.HP), player.StatsManager.GetMaxValue(EStatusType.HP))
                 .SetPlayerCharStamina(player.StatsManager.GetValue(EStatusType.Stamina), player.StatsManager.GetMaxValue(EStatusType.Stamina));

            frame.SetEnemyCharHP(enemy.StatsManager.GetValue(EStatusType.HP), enemy.StatsManager.GetMaxValue(EStatusType.HP))
                 .SetEnemyCharStamina(enemy.StatsManager.GetValue(EStatusType.Stamina), enemy.StatsManager.GetMaxValue(EStatusType.Stamina));
        }

        public void ShowCombatPreview(PlayerActorController player, EnemyActorController enemy)
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_COMBAT_DETAILS)
                         .AsFrame<UICombatDetailsFrame>()
                         .SetPlayerCharName(player.CharacterSO.CharacterName);
                         //.SetPlayerVit(player.StatsManager.GetValue(EStatusType.Vitality), player.ReadonlyDataHolder.GetVIT())
                         //.SetPlayerPow(player.StatPow.Value, player.ReadonlyDataHolder.GetPower())
                         //.SetPlayerAgi(player.StatAgi.Value, player.ReadonlyDataHolder.GetAgi())
                         //.SetPlayerInt(player.StatInt.Value, player.ReadonlyDataHolder.GetINT())
                         //.SetPlayerSta(player.StatStamina.Value, player.ReadonlyDataHolder.GetStamina())
                         //.SetEnemyName(enemy.ReadonlyDataHolder.GetCharacterName());
                         //.SetEnemyVit(enemy.StatVit.Value, enemy.ReadonlyDataHolder.GetVIT())
                         //.SetEnemyPow(enemy.StatPow.Value, enemy.ReadonlyDataHolder.GetPower())
                         //.SetEnemyAgi(enemy.StatAgi.Value, enemy.ReadonlyDataHolder.GetAgi())
                         //.SetEnemyInt(enemy.StatInt.Value, enemy.ReadonlyDataHolder.GetINT())
                         //.SetEnemySta(enemy.StatStamina.Value, enemy.ReadonlyDataHolder.GetStamina());
        }

        public void ShowCombatHUD(PlayerActorController player, EnemyActorController enemy)
        {
            GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_COMBAT_DETAILS);
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_COMBAT)
                         .AsFrame<UICombatFrame>()
                         .SetPlayerCharContent(player.CharacterSO.CharacterId, player.CharacterSO.Thumbnail)
                         .SetPlayerCharHP(player.StatsManager.GetValue(EStatusType.HP), player.StatsManager.GetMaxValue(EStatusType.HP))
                         .SetPlayerCharStamina(player.StatsManager.GetValue(EStatusType.Stamina), player.StatsManager.GetMaxValue(EStatusType.Stamina))
                         .SetEnemyCharContent(enemy.CharacterSO.CharacterId, enemy.CharacterSO.Thumbnail)
                         .SetEnemyCharHP(enemy.StatsManager.GetValue(EStatusType.HP), player.StatsManager.GetMaxValue(EStatusType.HP))
                         .SetEnemyCharStamina(enemy.StatsManager.GetValue(EStatusType.Stamina), player.StatsManager.GetMaxValue(EStatusType.Stamina));
        }

        public void ShowEnemyStatusEffects(EnemyActorController enemy)
        {
            GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                                   .AsFrame<UICombatFrame>()
                                   .LoadEnemyStatEffects(enemy.EffectsManager.ActiveStatEffects);
        }

        public void ShowPlayerStatusEffects(PlayerActorController player)
        {
            GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                       .AsFrame<UICombatFrame>()
                       .LoadPlayerStatEffects(player.EffectsManager.ActiveStatEffects);
        }

        public void ShowVictory(IFinishCombatHandler finishHandler)
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                .AsFrame<UIMessagePopup>()
                .SetContent("title_victory", "msg_victory", true, false,
                () =>
                {
                    if (finishHandler != null)
                    {
                        finishHandler.NavigateToScene();
                    }
                });
        }

        public void ShowDefeat(IFinishCombatHandler finishHandler)
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                .AsFrame<UIMessagePopup>()
                .SetContent("title_defeat", "msg_defeat", true, false,
                () =>
                {
                    if (finishHandler != null)
                    {
                        finishHandler.NavigateToScene();
                    }
                });
        }

        public void DisplayPlayerCards(IReadOnlyList<CardSO> cards, ICombatCardsService cardsService, float currentStamina)
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_COMBAT)
                                   .AsFrame<UICombatFrame>()
                                   .DisplayDrawnCards(cards, cardsService, currentStamina);
        }

        public void ClearAllUICards()
        {
            GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                                   .AsFrame<UICombatFrame>()
                                   .ClearAllCards();
        }
    }
}