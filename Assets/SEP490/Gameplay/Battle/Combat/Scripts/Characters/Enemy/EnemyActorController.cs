namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Tournament;
    using System.Collections.Generic;
    using System.ComponentModel;
    using UnityEngine;

    public class EnemyActorController : BaseCombatActor
    {
        [SerializeField] private EnemyIntentUIUpdater m_IntentUIUpdater;

        private EnemySO _enemySO;

        private IReadOnlyList<EnemyIntentSO> _intents = new List<EnemyIntentSO>();
        private Queue<IEnemyIntentStrategy> _intentQueue = new Queue<IEnemyIntentStrategy>();
        private IEnemyIntentStrategy _currentIntent;

        private SceneCombatController _battleManager;

        public EnemySO CharacterSO => _enemySO ??= _baseDataSO.ConvertAs<EnemySO>();

        public EnemyIntentUIUpdater IntentUIUpdater => m_IntentUIUpdater;

        /// <summary>
        /// Because the enemy only exists in battle, the system must manually create CharacterDataHolder for the enemy by itself.
        /// </summary>
        /// <param name="characterId"></param>
        public override void Initialize(BaseCharacterSO characterSO, SceneCombatController battleController)
        {
            _baseDataSO = characterSO;
            _enemySO = characterSO.ConvertAs<EnemySO>();
            _intents = _enemySO.IntentList;
            _battleManager = battleController;
            InitializeIntent();
            CreateStatsByProfile(new EnemyStatProfile());
            ICombatStatsInitializer initializer = new EnemyStatsInitializer(_enemySO);
            SetInitializer(initializer);
            InitializeStats();
        }

        private void InitializeIntent()
        {
            if (m_IntentUIUpdater == null)
            {
                GameObject intentUIPrefab = Resources.Load<GameObject>("Prefabs/UICanvas.Intent");
                if (intentUIPrefab != null)
                {
                    GameObject intentUIGO = Instantiate(intentUIPrefab, this.transform);
                    m_IntentUIUpdater = intentUIGO.GetComponent<EnemyIntentUIUpdater>();

                    if (m_IntentUIUpdater == null)
                    {
                        Debug.LogError($"[EnemyActorController.InitializeIntent fatal error] m_IntentUIUpdater is still null");
                    }
                }
                else
                {
                    Debug.LogError($"[EnemyActorController.InitializeIntent fatal error] Failed to load UICanvas.Intent prefab at Resources");
                }
            }
        }

        private void BuildIntentQueue()
        {
            _intentQueue.Clear();

            foreach (var intentSO in _enemySO.IntentList)
            {
                var intent = EnemyIntentFactory.CreateIntent(intentSO, this, _battleManager);
                _intentQueue.Enqueue(intent);
            }
        }

        public void PreviewNextIntent()
        {
            if (_intentQueue.Count == 0)
            {
                BuildIntentQueue();
            }

            _currentIntent = _intentQueue.Peek();

            if (_currentIntent == null)
            {
                Debug.LogError("[EnemyActorController.PreviewNextIntent fatal error] Current intent instance is null");
                return;
            }

            _currentIntent.Preview();
        }

        public void ExecuteTurn()
        {
            if (_intentQueue.Count == 0)
            {
                BuildIntentQueue();
            }

            _currentIntent = _intentQueue.Dequeue();

            TriggerTurnFlowEvent(ETurnFlowEvent.BeforeCardAction);
            EffectsManager.Trigger(ETurnFlowEvent.BeforeCardAction, this);

            _currentIntent.Execute(() =>
            {
                BaseCombatActor opponent = _battleManager.Player;

                opponent.CheckDeath();

                if (opponent.IsDead == false)
                {
                    _battleManager.ChangeToPlayerTurn();
                }
            });

            TriggerAfterCardResolved(_battleManager.Player);
        }

        public override void StartTurn()
        {
            IntentUIUpdater.ClearIntents();

            base.StartTurn();
        }

        #region Shield
        public void StackShield(float baseValue, float modifierValue)
        {
            StackShield(EStatusType.Attack, baseValue, modifierValue);
        }

        public float CalculateReceivedShield(float baseValue, float modifierValue)
        {
            return CalculateReceivedShield(EStatusType.Attack, modifierValue, baseValue);
        }

        #endregion

        #region Vfx Methods
        public void ExecuteVfxs(IReadOnlyList<SpawnVfxData> vfxList, BaseCombatActor opponent)
        {
            ExecuteVfxs(vfxList, opponent, null);
        }

        #endregion
    }
}