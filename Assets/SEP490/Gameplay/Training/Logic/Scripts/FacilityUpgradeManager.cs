namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using SEP490G69.GameSessions;
    using UnityEngine;

    public struct FacilityUpgradeRequirement
    {
        public int GoldCost;
        public EReputationRank RequiredRank;

        public FacilityUpgradeRequirement(int cost, EReputationRank rank)
        {
            GoldCost = cost;
            RequiredRank = rank;
        }
    }
    public enum EUpgradeResult { Success, MaxLevel, NotEnoughGold, RankTooLow, Error }

    public class FacilityUpgradeManager : MonoBehaviour, ISceneContext
    {
        private TrainingExerciseDAO _trainingDAO;
        private GameSessionDAO _sessionDAO;
        private GameTrainingController _trainingController;


        private readonly Dictionary<int, FacilityUpgradeRequirement> _upgradeConfigs = new Dictionary<int, FacilityUpgradeRequirement>()
        {
            { 2, new FacilityUpgradeRequirement(500, EReputationRank.F) },
            { 3, new FacilityUpgradeRequirement(1500, EReputationRank.D) },
            { 4, new FacilityUpgradeRequirement(3000, EReputationRank.C) },
            { 5, new FacilityUpgradeRequirement(5000, EReputationRank.B) }
        };

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
            LoadDAOs();
            _trainingController = GetComponent<GameTrainingController>();
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void LoadDAOs()
        {
            _trainingDAO = new TrainingExerciseDAO();
            _sessionDAO = new GameSessionDAO();
        }

        public EUpgradeResult TryUpgradeFacility(string sessionId, string exerciseId)
        {
            CharacterDataHolder holder = _trainingController.CharacterData;
            return TryUpgradeFacility(sessionId, exerciseId, holder);
        }

        public EUpgradeResult TryUpgradeFacility(string sessionId, string exerciseId, CharacterDataHolder character)
        {
            SessionTrainingExercise currentFacility = _trainingDAO.GetById(sessionId, exerciseId);
            if (currentFacility == null)
            {
                Debug.Log($"<color=red>[FacilityUpgradeManager error]</color> No facility with id {exerciseId} existed.");
                return EUpgradeResult.Error;
            }


            if (currentFacility.Level >= 5)
            {
                Debug.Log($"<color=red>[FacilityUpgradeManager]</color> Max level of facility {exerciseId} exceeded.");
                return EUpgradeResult.MaxLevel;
            }

            int nextLevel = currentFacility.Level + 1;
            var requirement = _upgradeConfigs[nextLevel];

            PlayerTrainingSession currentSession = _sessionDAO.GetById(sessionId);
            
            if (currentSession == null) return EUpgradeResult.Error;


            if (currentSession.CurrentGoldAmount < requirement.GoldCost)
            {
                return EUpgradeResult.NotEnoughGold;
            }


            EReputationRank currentRank = ReputationHelper.GetRankFromRP(character.GetRP());

            if (currentRank < requirement.RequiredRank)
            {
                return EUpgradeResult.RankTooLow;
            }


            currentSession.CurrentGoldAmount -= requirement.GoldCost;
            currentFacility.Level = nextLevel;

            bool isSessionSaved = _sessionDAO.Update(currentSession);
            bool isFacilitySaved = _trainingDAO.Update(currentFacility);

            if (isSessionSaved && isFacilitySaved)
            {
                Debug.Log($"Upgrade {exerciseId} to level {nextLevel} successful!");
                return EUpgradeResult.Success;
            }
            else
            {
                Debug.LogError("Failed to save upgrade data to database.");
                return EUpgradeResult.Error;
            }
        }

        public FacilityUpgradeRequirement? GetRequirementForNextLevel(int currentLevel)
        {
            if (currentLevel >= 5)
            {
                return null;
            }
            return _upgradeConfigs[currentLevel + 1];
        }

        public List<SessionTrainingExercise> GetAllExercises(string sessionId)
        {
            return _trainingDAO.GetAllBySessionId(sessionId);
        }
    }
}