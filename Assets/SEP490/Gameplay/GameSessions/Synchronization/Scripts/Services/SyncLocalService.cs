namespace SEP490G69.GameSessions
{
    using LiteDB;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Economy;
    using SEP490G69.Tournament;
    using SEP490G69.Training;
    using System;
    using System.Threading.Tasks;
    using UnityEngine;

    public class SyncLocalService 
    {
        private PlayerDataDAO _player = new();
        private GameSessionDAO _session = new();
        private PlayerCharacterRepository _char = new();
        private TrainingExerciseDAO _exercise = new();
        private GameCardsDAO _cards = new();
        private GameDeckDAO _deck = new();
        private GameInventoryDAO _inv = new();
        private GameShopDAO _shop = new();
        private TournamentProgressDAO _tour = new();

        public PlayerData GetPlayer(string playerId)
        {
            return _player.GetById(playerId);
        }

        public void UpdateLastSync(string playerId)
        {
            var p = _player.GetById(playerId);
            p.LastSyncTime = DateTime.UtcNow;
            _player.Update(p);
        }

        public OverrideCloudDataRequest BuildLocalPayload(string playerId, string sessionId)
        {
            var player = _player.GetById(playerId);
            var session = _session.GetById(sessionId);

            if (player == null || session == null)
            {
                return null;
            }

            DeviceInfo deviceInfo = new DeviceInfo
            {
                PlayerId = playerId ?? string.Empty,
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                DeviceType = SystemInfo.deviceType.ToString(),
            };

            PlayerInfoDTO playerInfo = new PlayerInfoDTO
            {
                PlayerId = playerId ?? string.Empty,
                PlayerName = player.PlayerName ?? string.Empty,
                PlayerEmail = player.PlayerEmail ?? string.Empty,
                LegacyPoints = player.LegacyPoints,
                CurrentRun = player.CurrentRun,
                LastSyncedDevice = deviceInfo,
                LastSyncedTime = DateTime.UtcNow,
            };

            return new OverrideCloudDataRequest
            {
                PlayerData = playerInfo,
                Session = DTOConverter.FromDB2DTO(session),
                Character = DTOConverter.FromDB2DTO(_char.GetCharacterData(sessionId, session.RawCharacterId)),
                Exercises = DTOConverter.FromDBList2DTO(_exercise.GetAllBySessionId(sessionId)),
                Cards = DTOConverter.FromDBList2DTO(_cards.GetAllBySessionId(sessionId)),
                Deck = DTOConverter.FromDB2DTO(_deck.GetById(sessionId)),
                ObtainedItems = DTOConverter.FromDBList2DTO(_inv.GetAllItems(sessionId)),
                ShopItems = DTOConverter.FromDBList2DTO(_shop.GetAll(sessionId)),
                Tournaments = DTOConverter.FromDBList2DTO(_tour.GetAllBySessionId(sessionId))
            };
        }

        public async Task<bool> OverrideLocal(string playerId, string sessionId, GetPlayerGameDataResponse cloud, SyncApiService api)
        {
            if (cloud?.Session == null)
            {
                return false;
            }

            var player = _player.GetById(playerId);

            if (player == null)
            {
                return false;
            }

            player.PlayerName = cloud.PlayerName;
            player.LegacyPoints = cloud.LegacyPoints;
            player.CurrentRun = cloud.CurrentRun;

            DateTime now = DateTime.UtcNow;
            player.LastSyncTime = now;

            await api.UpdateLastSync(playerId, now);

            bool success = LocalDBOrchestrator.Execute(db =>
            {
                db.BeginTrans();

                // DELETE OLD SESSION RECORD && INSERT NEW DATA.
                if (!_player.Update(player) ||
                    !ClearSession(db, sessionId) ||
                    !InsertAll(db, cloud))
                {
                    db.Rollback();
                    return false;
                }

                db.Commit();
                return true;
            });
            return success;
        }

        private bool ClearSession(LiteDatabase db, string sessionId)
        {
            return _session.DeleteById(db, sessionId)
                && _char.DeleteManyBySessionId(db, sessionId)
                && _exercise.DeleteAllBySessionId(db, sessionId)
                && _inv.DeleteManyBySessionId(db, sessionId)
                && _shop.DeleteManyBySessionId(db, sessionId)
                && _cards.DeleteAllBySessionId(db, sessionId)
                && _deck.Delete(db, sessionId)
                && _tour.DeleteAllBySessionId(db, sessionId);
        }

        private bool InsertAll(LiteDatabase db, GetPlayerGameDataResponse r)
        {
            return (_session.Insert(db, r.Session))
                && (_char.Insert(db, r.Character))
                && (_exercise.InsertMany(db, r.Exercises))
                && (_inv.InsertMany(db, r.ObtainedItems))
                && (_shop.InsertMany(db, r.ShopItems))
                && (_cards.InsertMany(db, r.Cards))
                && (_deck.Insert(db, r.Deck))
                && (_tour.InsertMany(db, r.TournamentProgressions));
        }
    }
}