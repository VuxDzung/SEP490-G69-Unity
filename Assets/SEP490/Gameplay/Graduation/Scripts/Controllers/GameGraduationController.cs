namespace SEP490G69
{
    using LiteDB;
    using System.Collections;
    using System.Collections.Generic;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Economy;
    using SEP490G69.GameSessions;
    using SEP490G69.Graduation;
    using SEP490G69.Tournament;
    using SEP490G69.Training;
    using System.Linq;
    using UnityEngine;
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Shared;

    public class GameGraduationController : MonoBehaviour, IGameContext
    {
        private const bool PERFORM_DB_ACTION = true;

        private IGraduationService _service;
        private ContextManager _contextManager;

        private EventManager _eventManager;
        private GameAuthManager _authManager;

        private PlayerDataDAO _playerDAO;

        private GameSessionDAO _sessionDAO;

        private PlayerCharacterRepository _characterRepo;
        private TrainingExerciseDAO _exerciseDAO = new();

        private GameCardsDAO _cardsDAO;
        private GameDeckDAO _deckDAO = new();

        private GameInventoryDAO _inventoryDAO;
        private GameShopDAO _shopDAO = new();

        private TournamentProgressDAO _tournamentsDAO = new();

        private GraduateRecordsDAO _graduateDAO;

        private void LoadDAOs()
        {
            _playerDAO = new PlayerDataDAO();
            _sessionDAO = new GameSessionDAO();
            _graduateDAO = new GraduateRecordsDAO();
            _characterRepo = new PlayerCharacterRepository();
            _cardsDAO = new GameCardsDAO();
            _inventoryDAO = new GameInventoryDAO();
        }

        private void LoadServices()
        {
            _service = new GraduationService();
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;

            _eventManager = _contextManager.ResolveGameContext<EventManager>();
            _authManager = _contextManager.ResolveGameContext<GameAuthManager>();

            LoadDAOs();
        }

        public void Graduate()
        {
            FadingController.Singleton.FadeIn2Out(1f, 1f, () =>
            {
                List<LoadTask> postLoadTasks = new List<LoadTask>
            {
                new LoadTask("Graduating", DelayGraduation),
            };
                SceneLoader.Singleton.StartLoad(GameConstants.SCENE_GRADUATION, null, postLoadTasks);
            });
        }

        private IEnumerator DelayGraduation()
        {
            yield return new WaitForSeconds(0.5f);
            PerformGraduate();
        }

        private void PerformGraduate()
        {
            PlayerData playerData = _playerDAO.GetById(_authManager.GetUserId());
            if (playerData == null)
            {
                return;
            }

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }
            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);
            if (sessionData == null)
            {
                return;
            }

            SessionCharacterData characterData = _characterRepo.GetCharacterData(sessionId, sessionData.RawCharacterId);
            if (characterData == null)
            {
                return;
            }

            string recordId = EntityIdConstructor.ConstructDBEntityId(playerData.PlayerId, playerData.CurrentRun.ToString());

            string title = "";
            string endingType = "";
            int cardCount = _cardsDAO.GetAllBySessionId(sessionId).Count;
            int relicCount = _inventoryDAO.GetAllItems(sessionId).Where(itm => itm.RawItemId.ToLower().StartsWith("rel_")).Count();
            int winTournamentCount = sessionData.WinTournamentCount;

            float rating = 0;
            int lpGained = 0;

            if (_service != null)
            {
                rating = _service.CalculateFinalRating(characterData, cardCount, relicCount);
                lpGained = _service.CalculateLPGained(rating);
            }

            string rank = GameConstants.GetEndGameRank(rating);

            EndGameRecordData endGameRecord = new EndGameRecordData
            {
                RecordId = recordId,
                PlayerId = playerData.PlayerId,
                RunChallegeCount = playerData.CurrentRun,
                RawCharacterId = characterData.RawCharacterId,
                Title = title,
                Rating = rating,
                ObtainedCardCount = cardCount,
                ObtainedRelicCount = relicCount,
                WinTournamentCount = winTournamentCount,
            };

            GameUIFrame graduationFrame = GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_GRADUAION);

            if (graduationFrame != null)
            {
                UIGraduationFrame graduateUIScreen = graduationFrame.AsFrame<UIGraduationFrame>();
                graduateUIScreen.SetLPGained(lpGained)
                                .SetCharacterTitle("")
                                .SetRank(rank)
                                .SetCardCount(cardCount)
                                .SetRelicCount(relicCount)
                                .SetTournamentWinCount(winTournamentCount);
            }

            if (PERFORM_DB_ACTION == true)
            {
                _graduateDAO.Upsert(endGameRecord);
                DeleteAllCurrentSessionData(sessionId);
            }
        }

        public List<EndGameRecordData> GetAllEndGameRecords()
        {
            PlayerData playerData = _playerDAO.GetById(_authManager.GetUserId());
            if (playerData == null)
            {
                return new List<EndGameRecordData>();
            }

            List<EndGameRecordData> records = _graduateDAO.GetAllByPlayerId(playerData.PlayerId);
            return records;
        }

        public void ShowHallOfFrame()
        {
            FadingController.Singleton.FadeIn2Out(0.5f, 0.5f, () =>
            {
                SceneLoader.Singleton.StartLoad(GameConstants.SCENE_HALL_OF_FAME, null, new List<LoadTask> { new LoadTask("Load data", DelayLoadHallOfFrame) });
            });
        }

        private IEnumerator DelayLoadHallOfFrame()
        {
            string playerId = _authManager.GetUserId();

            List<EndGameRecordData> playerRecords = _graduateDAO.GetAllByPlayerId(playerId);
            yield return new WaitForSeconds(0.5f);
            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_HALL_OF_FAME)
                         .AsFrame<UIHallOfFameFrame>()
                         .LoadRecords(playerRecords);
        }

        private bool DeleteAllCurrentSessionData(string sessionId)
        {
            bool success = LocalDBOrchestrator.Execute(db =>
            {
                db.BeginTrans();

                // DELETE OLD SESSION RECORD && INSERT NEW DATA.
                if (ClearSession(db, sessionId) == false)
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
            return _sessionDAO.DeleteById(db, sessionId)
                && _characterRepo.DeleteManyBySessionId(db, sessionId)
                && _exerciseDAO.DeleteAllBySessionId(db, sessionId)
                && _inventoryDAO.DeleteManyBySessionId(db, sessionId)
                && _shopDAO.DeleteManyBySessionId(db, sessionId)
                && _cardsDAO.DeleteAllBySessionId(db, sessionId)
                && _deckDAO.Delete(db, sessionId)
                && _tournamentsDAO.DeleteAllBySessionId(db, sessionId);
        }
    }
}