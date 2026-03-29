namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Economy;
    using SEP490G69.Exploration;
    using SEP490G69.GameSessions;
    using SEP490G69.Training;
    using UnityEngine;

    public class GameSnapshotController : MonoBehaviour, IGameContext
    {
        private ContextManager _contextManager;

        private string _sessionId;
        private SnapshotCheckpointDAO _snapshotDAO;

        private GameSessionDAO _sessionDAO;

        private PlayerCharacterRepository _characterRepo;
        private TrainingExerciseDAO _exercisesDAO;

        private GameDeckDAO _deckDAO;
        private GameCardsDAO _cardsDAO;

        private GameInventoryDAO _inventoryDAO;
        private GameShopDAO _shopDAO;

        private GameExploreLocationDAO _exploreLocationsDAO;

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;

            LoadDAOs();
        }

        private void LoadDAOs()
        {
            _sessionDAO = new GameSessionDAO();
            _snapshotDAO = new SnapshotCheckpointDAO();

            _characterRepo = new PlayerCharacterRepository();
            _exercisesDAO = new TrainingExerciseDAO();

            _deckDAO = new GameDeckDAO();
            _cardsDAO = new GameCardsDAO();

            _inventoryDAO = new GameInventoryDAO();
            _shopDAO = new GameShopDAO();

            _exploreLocationsDAO = new GameExploreLocationDAO();
        }

        public void RollbackToPrevCheckpoint(int currentWeek)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                _sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID, string.Empty);
            }

            if (string.IsNullOrEmpty(_sessionId))
            {
                Debug.LogError("[Rollback] SessionId is null");
                return;
            }

            string sessionId = _sessionId;
            int targetWeek = currentWeek;

            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);
            if (sessionData == null)
            {
                return;
            }

            // Lấy tất cả snapshot của session
            List<SessionSnapshotData> snapshots = _snapshotDAO.GetAllBySessionId(sessionId);

            if (snapshots == null || snapshots.Count == 0)
            {
                Debug.LogError("[Rollback] No snapshots found");
                return;
            }

            // Tìm snapshot gần nhất <= targetWeek
            SessionSnapshotData nearest = snapshots
                .Where(s =>
                {
                    string[] sp = s.SnapshotId.Split(':');
                    int week = int.Parse(sp[1]);
                    return week <= targetWeek;
                })
                .OrderByDescending(s => int.Parse(s.SnapshotId.Split(':')[1]))
                .FirstOrDefault();

            if (nearest == null)
            {
                Debug.LogError("[Rollback] No valid snapshot found. End game.");
                return;
            }

            if (!int.TryParse(nearest.SnapshotId.Split(':')[1], out int snapshotWeek))
            {
                return;
            }

            // ===== OVERWRITE DATABASE =====

            // Character
            if (nearest.CharacterData != null)
            {
                _characterRepo.Upsert(SnapshotModelConverter.FromSnapshot2Entity(nearest.CharacterData));
            }

            // Exercises
            _exercisesDAO.DeleteAllBySessionId(sessionId);
            _exercisesDAO.InsertMany(
                nearest.Exercises.Select(SnapshotModelConverter.FromSnapshot2Entity).ToList()
            );

            // Deck
            _deckDAO.Upsert(SnapshotModelConverter.FromSnapshot2Entity(nearest.Deck));

            // Cards
            _cardsDAO.DeleteAllBySessionId(sessionId);
            _cardsDAO.InsertMany(
                nearest.Cards.Select(SnapshotModelConverter.FromSnapshot2Entity).ToList()
            );

            // Inventory
            _inventoryDAO.DeleteManyBySessionId(sessionId);
            _inventoryDAO.InsertMany(
                nearest.InventoryItems.Select(SnapshotModelConverter.FromSnapshot2Entity).ToList()
            );

            // Shop
            _shopDAO.DeleteManyBySessionId(sessionId);
            _shopDAO.InsertMany(
                nearest.ShopItems.Select(SnapshotModelConverter.FromSnapshot2Entity).ToList()
            );

            // Explore
            _exploreLocationsDAO.DeleteManyById(sessionId);
            _exploreLocationsDAO.InsertMany(
                nearest.ExploreLocations.Select(SnapshotModelConverter.FromSnapshot2Entity).ToList()
            );

            sessionData.CurrentWeek = currentWeek;

            Debug.Log($"[Rollback] Done → {nearest.SnapshotId}");
        }

        public void CreateNewSnapshot(int currentWeek)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                _sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID, string.Empty);
            }
            if (string.IsNullOrEmpty(_sessionId))
            {
                return;
            }

            PlayerTrainingSession sessionData = _sessionDAO.GetById(_sessionId);

            if (sessionData == null)
            {
                return;
            }

            string snapshotCheckpointId = EntityIdConstructor.ConstructDBEntityId(_sessionId, currentWeek.ToString());

            SessionSnapshotData snapshotData = new SessionSnapshotData
            {
                SnapshotId = snapshotCheckpointId,
                SessionId = _sessionId
            };

            SessionCharacterData characterData = _characterRepo.GetCharacterData(_sessionId, sessionData.RawCharacterId);

            if (characterData == null)
            {
                Debug.LogError("[GameSnapshotController.CreateNewSnapshot error] No character data exist");
                return;
            }
            snapshotData.CharacterData = SnapshotModelConverter.FromEntity2Snapshot(characterData);

            List<SessionTrainingExercise> exercises = _exercisesDAO.GetAllBySessionId(_sessionId);
            if (exercises != null)
            {
                snapshotData.Exercises = exercises.Select(SnapshotModelConverter.FromEntity2Snapshot).ToList();
            }

            // ===== Deck =====
            SessionPlayerDeck deck = _deckDAO.GetById(_sessionId);
            if (deck != null)
            {
                snapshotData.Deck = SnapshotModelConverter.FromEntity2Snapshot(deck);
            }

            // ===== Cards =====
            List<SessionCardData> cards = _cardsDAO.GetAllBySessionId(_sessionId);
            if (cards != null)
            {
                snapshotData.Cards = cards
                    .Select(SnapshotModelConverter.FromEntity2Snapshot)
                    .ToList();
            }

            // ===== Inventory =====
            List<ItemData> items = _inventoryDAO.GetAllItems(_sessionId);
            if (items != null)
            {
                snapshotData.InventoryItems = items
                    .Select(SnapshotModelConverter.FromEntity2Snapshot)
                    .ToList();
            }

            // ===== Shop =====
            List<ShopItemData> shopItems = _shopDAO.GetAll(_sessionId);
            if (shopItems != null)
            {
                snapshotData.ShopItems = shopItems
                    .Select(SnapshotModelConverter.FromEntity2Snapshot)
                    .ToList();
            }

            // ===== Explore =====
            List<ExploreLocationData> locations = _exploreLocationsDAO.GetAllById(_sessionId);
            if (locations != null)
            {
                snapshotData.ExploreLocations = locations
                    .Select(SnapshotModelConverter.FromEntity2Snapshot)
                    .ToList();
            }

            // ===== Save =====
            _snapshotDAO.Upsert(snapshotData);
        }
    }
}