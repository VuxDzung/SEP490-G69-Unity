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

            frame.SetPlayerCharVit(player.GetCombatStatus(EStatusType.Vitality).Value,
                                   player.ReadonlyDataHolder.GetVIT());

            frame.SetEnemyCharVit(enemy.GetCombatStatus(EStatusType.Vitality).Value,
                                  enemy.ReadonlyDataHolder.GetVIT());
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
                         .SetPlayerCharVit(player.GetCombatStatus(EStatusType.Vitality).Value, player.ReadonlyDataHolder.GetVIT())
                         .SetPlayerCharStamina(player.GetCombatStatus(EStatusType.Stamina).Value, player.ReadonlyDataHolder.GetStamina())
                         .SetPlayerCharGauge(player.GetCurrentEnergyValue(), player.GetMaxEnergyValue())
                         .SetEnemyCharContent(enemy.ReadonlyDataHolder.GetRawId(), enemy.ReadonlyDataHolder.GetAvatar())
                         .SetEnemyCharVit(enemy.GetCombatStatus(EStatusType.Vitality).Value, enemy.ReadonlyDataHolder.GetVIT())
                         .SetEnemyCharStamina(enemy.GetCombatStatus(EStatusType.Stamina).Value, enemy.ReadonlyDataHolder.GetStamina())
                         .SetEnemyCharGauge(enemy.GetCurrentEnergyValue(), enemy.GetMaxEnergyValue());
        }

        public void ShowVictory()
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                .AsFrame<UIMessagePopup>()
                .SetContent("title_victory", "msg_victory", true, false,
                () => SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TOURNAMENT));
        }

        public void ShowDefeat()
        {
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP)
                .AsFrame<UIMessagePopup>()
                .SetContent("title_defeat", "msg_defeat", true, false,
                () => SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TOURNAMENT));
        }
    }
}