namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class BaseCombatFrame : GameUIFrame
    {
        private SceneCombatController _combatController;
        protected SceneCombatController CombatController
        {
            get
            {
                if (_combatController == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext(out _combatController);
                }
                return _combatController;
            }
        }
    }
}