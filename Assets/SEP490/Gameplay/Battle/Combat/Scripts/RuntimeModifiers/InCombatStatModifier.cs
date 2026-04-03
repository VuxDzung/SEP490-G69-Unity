namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;

    public class InCombatStatModifier 
    {
        public CombatStatModifierSO ModifierSO {  get; private set; }
        private readonly HashSet<string> _owners = new HashSet<string>();
        private readonly StatusEffectManager _effectsManager;
        public HashSet<string> Owners => _owners;

        public InCombatStatModifier(CombatStatModifierSO modifierSO, StatusEffectManager effectManager)
        {
            ModifierSO = modifierSO;
            _effectsManager = effectManager;
        }

        public void AddOwner(string ownerId)
        {
            if (!_owners.Contains(ownerId))
            {
                _owners.Add(ownerId);
            }
        }

        public void RemoveOwner(string ownerId)
        {
            if (_owners.Contains(ownerId))
            {
                _owners.Remove(ownerId);
            }
        }

        public int OwnerStack
        {
            get
            {
                int totalStack = 0;

                foreach (var id in _owners)
                {
                    var effect = _effectsManager.GetById(id);
                    if (effect != null && effect.Data.StackableValue == true)
                    {
                        totalStack += effect.Stack;
                    }
                }

                return totalStack;
            }
        }
    }
}