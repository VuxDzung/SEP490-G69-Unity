namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class CharacterEnergyProcessor : MonoBehaviour
    {
        private BaseBattleCharacterController _owner;
        private EnergyTurnBar _bar;
        private bool _paused;

        public void SetOwner(BaseBattleCharacterController owner)
        {
            _owner = owner;
        }

        public void Initialize(float agi)
        {
            _bar = new EnergyTurnBar(new AgiBasedChargeStrategy(10f, agi));
        }

        public bool UpdateEnergy(float deltaTime)
        {
            if (_paused || _bar == null || _bar.IsFull) return false;

            _bar.Update(deltaTime);
            return _bar.IsFull;
        }

        public void ResetEnergy() => _bar?.Reset();
        public void PauseEnergy() => _paused = true;
    }
}