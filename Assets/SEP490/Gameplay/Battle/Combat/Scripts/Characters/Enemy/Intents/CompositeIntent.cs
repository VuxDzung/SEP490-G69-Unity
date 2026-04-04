namespace SEP490G69.Battle.Combat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CompositeIntent : IEnemyIntentStrategy
    {
        private List<IEnemyIntentStrategy> _actions = new();

        public EIntentAction IntentType => EIntentAction.None;

        public void Add(IEnemyIntentStrategy action)
        {
            _actions.Add(action);
        }

        public void Initialize(EnemyActorController owner, EnemyIntentSO data, SceneCombatController battleManager)
        {
            foreach (var action in _actions)
            {
                action.Initialize(owner, data, battleManager);
            }
        }

        public void Preview()
        {
            foreach (var action in _actions)
            {
                action.Preview();
            }
        }

        public void Execute(Action onCompleted)
        {
            ExecuteNext(0, onCompleted);
        }

        private void ExecuteNext(int index, Action onComplete)
        {
            if (index >= _actions.Count)
            {
                onComplete?.Invoke();
                return;
            }

            _actions[index].Execute(() =>
            {
                ExecuteNext(index + 1, onComplete);
            });
        }

        public BaseCombatActor SelectTarget()
        {
            return _actions.LastOrDefault()?.SelectTarget();
        }
    }
}