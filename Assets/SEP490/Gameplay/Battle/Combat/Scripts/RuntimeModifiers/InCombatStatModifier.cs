namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;

    public class InCombatStatModifier 
    {
        public CombatStatModifierSO ModifierSO {  get; private set; }
        private readonly List<string> _owners = new List<string>();

        public IReadOnlyList<string> OwnerIds => _owners;

        public InCombatStatModifier(CombatStatModifierSO modifierSO)
        {
            ModifierSO = modifierSO;
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
    }
}