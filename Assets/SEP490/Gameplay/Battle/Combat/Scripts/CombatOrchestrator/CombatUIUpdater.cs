namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Shared;

    public class CombatUIUpdater
    {
        public void UpdateEnergy(PlayerBattleCharaterController player, EnemyCombatController enemy)
        {
            UICombatFrame frame = GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                                                         .AsFrame<UICombatFrame>();

            frame.SetPlayerCharGauge(player.GetCurrentEnergyValue(), player.GetMaxEnergyValue());
            frame.SetEnemyCharGauge(enemy.GetCurrentEnergyValue(), enemy.GetMaxEnergyValue());
        }

        public void UpdateStats(PlayerBattleCharaterController player, EnemyCombatController enemy)
        {
            UICombatFrame frame = GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                                               .AsFrame<UICombatFrame>();

            frame.SetPlayerCharVit(player.GetCombatStatus(EStatusType.HP).Value, player.GetMaxStatus(EStatusType.HP))
                 .SetPlayerCharStamina(player.GetCombatStatus(EStatusType.Stamina).Value, player.GetMaxStatus(EStatusType.Stamina));

            frame.SetEnemyCharVit(enemy.GetCombatStatus(EStatusType.HP).Value, enemy.GetMaxStatus(EStatusType.HP))
                 .SetEnemyCharStamina(enemy.GetCombatStatus(EStatusType.Stamina).Value, enemy.GetMaxStatus(EStatusType.Stamina));
        }

        public void ShowCombatPreview(PlayerBattleCharaterController player, EnemyCombatController enemy)
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_COMBAT_DETAILS)
                         .AsFrame<UICombatDetailsFrame>()
                         .SetPlayerCharName(player.ReadonlyDataHolder.GetCharacterName())
                         .SetPlayerVit(player.StatVit.Value, player.ReadonlyDataHolder.GetVIT())
                         .SetPlayerPow(player.StatPow.Value, player.ReadonlyDataHolder.GetPower())
                         .SetPlayerAgi(player.StatAgi.Value, player.ReadonlyDataHolder.GetAgi())
                         .SetPlayerInt(player.StatInt.Value, player.ReadonlyDataHolder.GetINT())
                         .SetPlayerSta(player.StatStamina.Value, player.ReadonlyDataHolder.GetStamina())
                         .SetEnemyName(enemy.ReadonlyDataHolder.GetCharacterName())
                         .SetEnemyVit(enemy.StatVit.Value, enemy.ReadonlyDataHolder.GetVIT())
                         .SetEnemyPow(enemy.StatPow.Value, enemy.ReadonlyDataHolder.GetPower())
                         .SetEnemyAgi(enemy.StatAgi.Value, enemy.ReadonlyDataHolder.GetAgi())
                         .SetEnemyInt(enemy.StatInt.Value, enemy.ReadonlyDataHolder.GetINT())
                         .SetEnemySta(enemy.StatStamina.Value, enemy.ReadonlyDataHolder.GetStamina());
        }

        public void ShowCombatHUD(PlayerBattleCharaterController player, EnemyCombatController enemy)
        {
            GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_COMBAT_DETAILS);
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_COMBAT)
                         .AsFrame<UICombatFrame>()
                         .SetPlayerCharContent(player.ReadonlyDataHolder.GetRawId(), player.ReadonlyDataHolder.GetAvatar())
                         .SetPlayerCharVit(player.GetCombatStatus(EStatusType.HP).Value, player.GetMaxStatus(EStatusType.HP))
                         .SetPlayerCharStamina(player.GetCombatStatus(EStatusType.Stamina).Value, player.GetMaxStatus(EStatusType.Stamina))
                         .SetPlayerCharGauge(player.GetCurrentEnergyValue(), player.GetMaxEnergyValue())
                         .SetEnemyCharContent(enemy.ReadonlyDataHolder.GetRawId(), enemy.ReadonlyDataHolder.GetAvatar())
                         .SetEnemyCharVit(enemy.GetCombatStatus(EStatusType.HP).Value, player.GetMaxStatus(EStatusType.HP))
                         .SetEnemyCharStamina(enemy.GetCombatStatus(EStatusType.Stamina).Value, player.GetMaxStatus(EStatusType.Stamina))
                         .SetEnemyCharGauge(enemy.GetCurrentEnergyValue(), enemy.GetMaxEnergyValue());
        }

        public void ShowEnemyStatusEffects(EnemyCombatController enemy)
        {
            GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                                   .AsFrame<UICombatFrame>()
                                   .LoadEnemyStatEffects(enemy.StatEffectManager.ActiveStatEffects);
        }

        public void ShowPlayerStatusEffects(PlayerBattleCharaterController player)
        {
            GameUIManager.Singleton.GetFrame(GameConstants.FRAME_ID_COMBAT)
                       .AsFrame<UICombatFrame>()
                       .LoadEnemyStatEffects(player.StatEffectManager.ActiveStatEffects);
        }

        public void ShowVictory(string combatType)
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                .AsFrame<UIMessagePopup>()
                .SetContent("title_victory", "msg_victory", true, false,
                () =>
                {
                    switch (combatType)
                    {
                        case GameConstants.COMBAT_TYPE_TOURNAMENT:
                            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TOURNAMENT);
                            break;
                        case GameConstants.COMBAT_TYPE_EXPLORATION:
                            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_EXPLORATION);
                            break;
                    }
                });
        }

        public void ShowDefeat(string combatType)
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                .AsFrame<UIMessagePopup>()
                .SetContent("title_defeat", "msg_defeat", true, false,
                () =>
                {
                    switch (combatType)
                    {
                        case GameConstants.COMBAT_TYPE_TOURNAMENT:
                            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TOURNAMENT);
                            break;
                        case GameConstants.COMBAT_TYPE_EXPLORATION:
                            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_EXPLORATION);
                            break;
                        case GameConstants.COMBAT_TYPE_TESTING:
                            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
                            break;
                    }
                });
        }
    }
}