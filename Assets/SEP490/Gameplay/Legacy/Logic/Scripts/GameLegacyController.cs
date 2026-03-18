namespace SEP490G69.Legacy
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameLegacyController : MonoBehaviour, IGameContext
    {
        private ContextManager _contextManager;

        private GameLegacyDAO _legacyDAO;
        private PlayerDataDAO _playerDAO;
        private LegacyStatConfigSO _legacyStatsConfig;

        public LegacyStatConfigSO LegacyStatsConfig
        {
            get
            {
                if (_legacyStatsConfig == null)
                {
                    _legacyStatsConfig = ContextManager.Singleton.GetDataSO<LegacyStatConfigSO>();
                }
                return _legacyStatsConfig;
            }
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;
        }

        private void Awake()
        {
            _legacyDAO = new GameLegacyDAO();
            _playerDAO = new PlayerDataDAO();
        }

        public IReadOnlyList<LegacyStatDataHolder> GetAllPlayerLegacyStats(string playerId)
        {
            List<LegacyStatDataHolder> holders = new List<LegacyStatDataHolder>();
            foreach (var so in LegacyStatsConfig.LegacyStats)
            {
                LegacyStatData legacyStat = _legacyDAO.GetById(playerId, so.LegacyStatId);
            
                LegacyStatDataHolder holder = new LegacyStatDataHolder.Builder()
                                                                      .WithData(legacyStat)
                                                                      .WithSO(so)
                                                                      .Build();
                holders.Add(holder);
            }
            return holders;
        }

        public int GetCurrentLP(string playerId)
        {
            PlayerData playerData = _playerDAO.GetById(playerId);

            if (playerData == null)
            {
                Debug.LogError($"[GameLegacyController.GetCurrentLP fatal error] No player account with id {playerId} exists in database");
                return 0;
            }

            return playerData.LegacyPoints;
        }

        public LegacyStatDataHolder GetLegacyStatByRawId(string playerId, string rawId)
        {
            LegacyStatSO dataSO = LegacyStatsConfig.LegacyStats.FirstOrDefault(ls => ls.LegacyStatId.Equals(rawId));

            if (dataSO == null)
            {
                Debug.LogError($"[GameLegacyController.GetLegacyStatByRawId error] No LegacyStatSO with id {rawId} exists in config");
                return null;
            }

            LegacyStatData data = _legacyDAO.GetById(playerId, rawId);

            if (data == null)
            {
                Debug.LogError($"[GameLegacyController.GetLegacyStatByRawId error] No LegacyStatData with raw id {rawId} of player {playerId} exists in database");
                return null;
            }

            return new LegacyStatDataHolder.Builder().WithData(data).WithSO(dataSO).Build();
        }

        public bool TryUpgradeLegacyStat(string entityId)
        {
            LegacyStatData data = _legacyDAO.GetById(entityId);

            if (data == null)
            {
                Debug.LogError($"[GameLegacyController.TryUpgradeLegacyStat error] No LegacyStatData with entity id {entityId} exists in database");
                return false;
            }
            if (data.Level > GameConstants.LEGACY_STATS_MAX_LV)
            {
                return false;
            }

            data.Level++;
            
            if (data.Level > GameConstants.LEGACY_STATS_MAX_LV)
            {
                data.Level = GameConstants.LEGACY_STATS_MAX_LV;
            }

            _legacyDAO.Update(data);

            return _legacyDAO.Update(data);
        }

        public LegacyStatDataHolder GetByType(string playerId, EStatusType statType)
        {
            LegacyStatSO legacySOStat = LegacyStatsConfig.LegacyStats.FirstOrDefault(ls => ls.StatType == statType);

            if (legacySOStat == null)
            {
                return null;
            }

            LegacyStatData data = _legacyDAO.GetById(playerId, legacySOStat.LegacyStatId);

            if (data == null)
            {
                return null;
            }

            return new LegacyStatDataHolder.Builder()
                                           .WithData(data)
                                           .WithSO(legacySOStat).Build();
        }
    }
}