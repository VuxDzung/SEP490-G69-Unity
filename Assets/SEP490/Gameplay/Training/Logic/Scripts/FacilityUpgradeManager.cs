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

        private TrainingExerciseConfigSO _exercisesConfig;
        private TrainingExerciseConfigSO ExercisesConfig
        {
            get
            {
                if (this._exercisesConfig == null)
                {
                    _exercisesConfig = ContextManager.Singleton.GetDataSO<TrainingExerciseConfigSO>();
                }
                return this._exercisesConfig;
            }
        }


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

        public EUpgradeResult TryUpgradeFacility(string sessionId, string rawExerciseId)
        {
            CharacterDataHolder holder = _trainingController.CharacterData;
            return TryUpgradeFacility(sessionId, rawExerciseId, holder);
        }

        public EUpgradeResult TryUpgradeFacility(string sessionId, string rawExerciseId, CharacterDataHolder character)
        {
            SessionTrainingExercise currentFacility = _trainingDAO.GetById(sessionId, rawExerciseId);
            if (currentFacility == null)
            {
                Debug.Log($"<color=red>[FacilityUpgradeManager error]</color> No facility with id {rawExerciseId} existed.");
                return EUpgradeResult.Error;
            }


            if (currentFacility.Level >= 5)
            {
                Debug.Log($"<color=red>[FacilityUpgradeManager]</color> Max level of facility {rawExerciseId} exceeded.");
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
                Debug.Log($"Upgrade {rawExerciseId} to level {nextLevel} successful!");
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

        public List<TrainingExerciseDataHolder> GetAllExercises(string sessionId)
        {
            List<TrainingExerciseDataHolder> holderList = new List<TrainingExerciseDataHolder>();
            List<SessionTrainingExercise> exerciseDataList = _trainingDAO.GetAllBySessionId(sessionId);

            foreach (var exercise in exerciseDataList)
            {
                TrainingExerciseSO exerciseSO = ExercisesConfig.GetExercise(exercise.ExerciseId);
                if (exerciseSO != null)
                {
                    TrainingExerciseDataHolder holder = new TrainingExerciseDataHolder.Builder()
                                                                                      .WithExerciseSO(exerciseSO)
                                                                                      .WithSessionTrainingData(exercise)
                                                                                      .Build();
                    holderList.Add(holder);
                }
            }

            return holderList;
        }
    }
}