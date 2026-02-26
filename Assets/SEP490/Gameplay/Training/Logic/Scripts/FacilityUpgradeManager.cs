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

    public class FacilityUpgradeManager
    {
        private TrainingExerciseDAO _trainingDAO;
        private GameSessionDAO _sessionDAO; 


        private readonly Dictionary<int, FacilityUpgradeRequirement> _upgradeConfigs = new Dictionary<int, FacilityUpgradeRequirement>()
        {
            { 2, new FacilityUpgradeRequirement(500, EReputationRank.F) },
            { 3, new FacilityUpgradeRequirement(1500, EReputationRank.D) },
            { 4, new FacilityUpgradeRequirement(3000, EReputationRank.C) },
            { 5, new FacilityUpgradeRequirement(5000, EReputationRank.B) }
        };

        public FacilityUpgradeManager(TrainingExerciseDAO trainingDao, GameSessionDAO sessionDao)
        {
            _trainingDAO = trainingDao;
            _sessionDAO = sessionDao;
        }

        public enum EUpgradeResult { Success, MaxLevel, NotEnoughGold, RankTooLow, Error }

        public EUpgradeResult TryUpgradeFacility(string sessionId, string exerciseId, CharacterDataHolder character)
        {
            SessionTrainingExercise currentFacility = _trainingDAO.GetByIdAndSessionId(sessionId, exerciseId);
            if (currentFacility == null) return EUpgradeResult.Error;


            if (currentFacility.Level >= 5)
            {
                return EUpgradeResult.MaxLevel;
            }

            int nextLevel = currentFacility.Level + 1;
            var requirement = _upgradeConfigs[nextLevel];

            PlayerTrainingSession currentSession = _sessionDAO.GetSession(sessionId);
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

            bool isSessionSaved = _sessionDAO.UpdateSession(currentSession);
            bool isFacilitySaved = _trainingDAO.UpdateTrainingExercise(currentFacility);

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
            if (currentLevel >= 5) return null;
            return _upgradeConfigs[currentLevel + 1];
        }
    }
}