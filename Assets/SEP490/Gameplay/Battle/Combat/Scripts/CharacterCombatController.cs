namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using UnityEngine;

    public abstract class CharacterCombatController : MonoBehaviour, IDamageable
    {
        private CharacterConfigSO _characterConfig;
        protected CharacterConfigSO CharacterConfig
        {
            get
            {
                if (_characterConfig == null)
                {
                    _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
                }
                return _characterConfig;
            }
        }

        private CardConfigSO _cardConfig;
        protected CardConfigSO CardConfig
        {
            get
            {
                if (_cardConfig == null)
                {
                    _cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();
                }
                return _cardConfig;
            }
        }

        public abstract void Initialize(string characterId);

        public virtual void ReceiveCardEffect(CardSO cardSO)
        {
            switch (cardSO.ActionType)
            {
                case EActionType.Attack:
                    OnHandleAttackAction(cardSO);
                    break;
                case EActionType.Effect:
                    OnHandleEffectAction(cardSO);
                    break;
                case EActionType.StatRecover:
                    OnHandleStatRecoverAction(cardSO);
                    break;
                case EActionType.HPRecover:
                    OnHandleVitRecoverAction(cardSO);
                    break;
                case EActionType.Other:
                    OnHandleOtherAction(cardSO);
                    break;
                default:
                    Debug.LogError($"Unsupported card action type {cardSO.ActionType.ToString()}");
                    break;
            }
        }

        protected virtual void OnHandleAttackAction(CardSO cardSO)
        {

        }
        protected virtual void OnHandleEffectAction(CardSO cardSO)
        {

        }
        protected virtual void OnHandleStatRecoverAction(CardSO cardSO)
        {

        }
        protected virtual void OnHandleVitRecoverAction(CardSO cardSO)
        {

        }
        protected virtual void OnHandleOtherAction(CardSO cardSO)
        {

        }
    }
}