namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Tournament;
    using System.Collections.Generic;

    public class EnemyActorController : BaseCombatActor
    {
        private EnemySO _enemySO;

        private IReadOnlyList<EnemyIntentSO> _intents = new List<EnemyIntentSO>();
        private Queue<IEnemyIntentStrategy> _intentQueue = new();
        private IEnemyIntentStrategy _currentIntent;

        private int _currentIntentIndex = 0;

        public EnemySO CharacterSO => _enemySO ??= _baseDataSO.ConvertAs<EnemySO>();

        /// <summary>
        /// Because the enemy only exists in battle, the system must manually create CharacterDataHolder for the enemy by itself.
        /// </summary>
        /// <param name="characterId"></param>
        public override void Initialize(BaseCharacterSO characterSO)
        {
            _baseDataSO = characterSO;
            _enemySO = characterSO.ConvertAs<EnemySO>();
            _intents = _enemySO.IntentList;

            CreateStatsByProfile(new EnemyStatProfile());
            ICombatStatsInitializer initializer = new EnemyStatsInitializer(_enemySO);
            SetInitializer(initializer);
            InitializeStats();
        }

        public void PerformIntent()
        {

        }

        private void BuildIntentQueue()
        {
            _intentQueue.Clear();

            foreach (var intentSO in _enemySO.IntentList)
            {
                var intent = EnemyIntentFactory.CreateIntent(intentSO, this);
                _intentQueue.Enqueue(intent);
            }
        }

        public void PreviewNextIntent()
        {
            if (_intentQueue.Count == 0)
                BuildIntentQueue();

            _currentIntent = _intentQueue.Peek();
            _currentIntent.Preview();
        }

        public void ExecuteTurn()
        {
            if (_intentQueue.Count == 0)
                BuildIntentQueue();

            _currentIntent = _intentQueue.Dequeue();

            TriggerTurnFlowEvent(ETurnFlowEvent.BeforeCardAction);
            EffectsManager.Trigger(ETurnFlowEvent.BeforeCardAction, this);

            _currentIntent.Execute();

            TriggerAfterCardResolved(_currentIntent.SelectTarget());

            EndCurrentTurn();
        }

        #region Shield
        public void StackShield(float baseValue, float modifierValue)
        {
            StackShield(EStatusType.Attack, baseValue, modifierValue);
        }

        #endregion
    }
}