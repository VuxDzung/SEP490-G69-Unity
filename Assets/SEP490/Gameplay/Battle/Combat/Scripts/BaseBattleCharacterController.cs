namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class BaseBattleCharacterController : MonoBehaviour, ICardEffectReceiver
    {
        #region Events

        public event Action<BaseBattleCharacterController> OnEnergyFull;

        #endregion

        #region Configs (Lazy Loaded)

        private CharacterConfigSO _characterConfig;
        protected CharacterConfigSO CharacterConfig =>
            _characterConfig ??= ContextManager.Singleton.GetDataSO<CharacterConfigSO>();

        private CardConfigSO _cardConfig;
        protected CardConfigSO CardConfig =>
            _cardConfig ??= ContextManager.Singleton.GetDataSO<CardConfigSO>();

        #endregion

        #region Fields

        private bool _isBarPaused;
        private EnergyTurnBar _energyTurnBar;

        private CharacterDataHolder _readonlyDataHolder;
        private CharacterDataHolder _runtimeDataHolder;

        private readonly List<RuntimeStatusEffect> _activeStatuses = new();

        private List<CardSO> _deckPool = new List<CardSO>();
        private List<CardSO> _discardPool = new List<CardSO>();
        private List<CardSO> _currentDrawPool = new List<CardSO>();
        private CardSO _selectedCard = null;

        #endregion

        #region Properties

        public CharacterDataHolder ReadonlyDataHolder => _readonlyDataHolder;
        public CharacterDataHolder CharacterDataHolder => _runtimeDataHolder;
        public IReadOnlyList<RuntimeStatusEffect> ActiveStatuses => _activeStatuses;
        public CardSO SelectedCard => _selectedCard;

        #endregion

        #region Initialization

        public abstract void Initialize(BaseCharacterSO characterSO);

        public void SetCharacterDataHolder(CharacterDataHolder holder)
        {
            _runtimeDataHolder = holder;
        }

        public void SetReadonlyDataHolder(CharacterDataHolder holder)
        {
            _readonlyDataHolder = holder;
        }

        #endregion

        // =========================================================
        // CARD EFFECT PIPELINE
        // =========================================================
        #region Card Effect Pipeline

        public virtual void ReceiveCardEffect(
            BaseBattleCharacterController source,
            BaseBattleCharacterController target)
        {
            ExecuteMainAction(SelectedCard, source, target);
            ApplyStatuses(SelectedCard.StatusGains, source, target);
            ApplyStatuses(SelectedCard.StatusInflicts, source, target);
        }

        private void ExecuteMainAction(
            CardSO cardSO,
            BaseBattleCharacterController source,
            BaseBattleCharacterController target)
        {
            switch (cardSO.ActionType)
            {
                case EActionType.Attack:
                    target.HandleAttack(cardSO, source);
                    break;

                case EActionType.StatRecover:
                    source.HandleStatRecovery(cardSO);
                    break;

                case EActionType.HPRecover:
                    source.HandleVitRecover(cardSO);
                    break;
            }
        }

        private void ApplyStatuses(
            StatusEffectSO[] statuses,
            BaseBattleCharacterController source,
            BaseBattleCharacterController target)
        {
            if (statuses == null) return;

            foreach (var status in statuses)
            {
                var receiver = status.TargetType == ETargetType.Self ? source : target;
                receiver.AddStatus(status);
            }
        }

        public virtual void StartNewTurn()
        {
            OnTurnEnd();

            foreach (CardSO card in _currentDrawPool)
            {
                if (card != _selectedCard)
                    _discardPool.Add(card);
            }

            _selectedCard = null;

            ResetCharge();
        }
        #endregion

        // =========================================================
        // ACTION HANDLERS
        // =========================================================
        #region Action Handlers

        protected virtual void HandleAttack(CardSO cardSO, BaseBattleCharacterController attacker)
        {
            float finalDamage = DamageCalculator.Calculate(
                attacker,
                this,
                cardSO.BaseDmg,
                cardSO.DamageType
            );

            ReceiveDamage(finalDamage);
        }

        protected virtual void HandleStatRecovery(CardSO cardSO)
        {
            if (cardSO.RecoverModifiers == null) return;

            foreach (var modifier in cardSO.RecoverModifiers)
            {
                ApplyStatusDelta(modifier);
            }
        }

        protected virtual void HandleVitRecover(CardSO cardSO)
        {
            float currentVit = CharacterDataHolder.GetVIT();
            float final = currentVit + cardSO.BaseDmg;

            CharacterDataHolder.SetStatus(EStatusType.Vitality, final);
        }

        #endregion

        // =========================================================
        // DAMAGE SYSTEM
        // =========================================================
        #region Damage System

        private void ReceiveDamage(float dmg)
        {
            if (dmg <= 0) return;

            float finalDamage = ApplyIncomingDamageModifiers(dmg);
            ApplyVitalityLoss(finalDamage);
            TriggerAfterDamageHooks(finalDamage);
            CheckDeath();
        }

        private float ApplyIncomingDamageModifiers(float dmg)
        {
            float modified = dmg;

            foreach (var status in _activeStatuses.ToList())
                modified = status.ModifyIncomingDamage(modified);

            return Mathf.Max(0, modified);
        }

        private void ApplyVitalityLoss(float damage)
        {
            float currentVit = CharacterDataHolder.GetVIT();
            float remaining = currentVit - damage;

            CharacterDataHolder.SetStatus(EStatusType.Vitality, remaining);

            Debug.Log($"{CharacterDataHolder.GetCharacterName()} took {damage} damage");
        }

        private void TriggerAfterDamageHooks(float damage)
        {
            foreach (var status in _activeStatuses.ToList())
                status.OnAfterReceiveDamage(damage);
        }

        private void CheckDeath()
        {
            if (CharacterDataHolder.GetVIT() > 0) return;

            Debug.Log($"{CharacterDataHolder.GetCharacterName()} has died.");
            PauseBar();
        }

        #endregion

        // =========================================================
        // STATUS SYSTEM
        // =========================================================
        #region Status System

        public void AddStatus(StatusEffectSO effectSO)
        {
            var existing = GetStatusById(effectSO.EffectId);

            if (existing != null)
            {
                existing.Amount++;
                return;
            }

            var runtime = new RuntimeStatusEffect(effectSO, this);
            _activeStatuses.Add(runtime);
            runtime.OnApply();
        }

        public void RemoveStatus(RuntimeStatusEffect effect)
        {
            _activeStatuses.Remove(effect);
        }

        public RuntimeStatusEffect GetStatusById(string id)
        {
            return _activeStatuses.FirstOrDefault(x => x.Data.EffectId == id);
        }

        public void OnTurnStart()
        {
            foreach (var status in _activeStatuses.ToList())
                status.OnTurnStart();
        }

        public void OnTurnEnd()
        {
            foreach (var status in _activeStatuses.ToList())
                status.OnTurnEnd();
        }

        public void ApplyStatusDelta(StatusModifierSO modifierSO)
        {
            float current = CharacterDataHolder.GetStatus(modifierSO.StatType);
            float delta = modifierSO.GetDelta(current);

            CharacterDataHolder.ModifyStat(modifierSO.StatType, delta);
        }

        public void ApplyStatusEffect(StatusEffectSO effectSO)
        {
            float current = CharacterDataHolder.GetStatus(effectSO.StatType);
            float delta = effectSO.GetDelta(current);

            CharacterDataHolder.ModifyStat(effectSO.StatType, delta);
        }

        #endregion

        // =========================================================
        // ENERGY SYSTEM
        // =========================================================
        #region Energy System

        public void InitializeEnergySystem()
        {
            var strategy = new AgiBasedChargeStrategy(
                GameConstants.BASE_FILL_SPEED,
                _runtimeDataHolder.GetAgi()
            );

            _energyTurnBar = new EnergyTurnBar(strategy);
        }

        public void UpdateCharge(float deltaTime)
        {
            if (_energyTurnBar == null || _isBarPaused || _energyTurnBar.IsFull)
                return;

            _energyTurnBar.Update(deltaTime);

            if (_energyTurnBar.IsFull)
                OnEnergyFull?.Invoke(this);
        }

        public void ResetCharge()
        {
            _energyTurnBar?.Reset();
        }

        public float GetCurrentEnergyValue() => _energyTurnBar.CurrentValue;
        public float GetMaxEnergyValue() => _energyTurnBar.MaxValue;
        public void PauseBar() => _isBarPaused = true;
        public void UnpauseBar() => _isBarPaused = false;

        #endregion

        protected void InitializeDeck(string[] cardIdArray)
        {
            if (cardIdArray == null || cardIdArray.Length == 0)
            {
                Debug.LogError("Deck is empty");
                return;
            }

            _deckPool.Clear();
            _discardPool.Clear();
            _currentDrawPool.Clear();

            foreach (var id in cardIdArray)
            {
                CardSO card = CardConfig.GetCardById(id);
                if (card != null)
                    _deckPool.Add(card);
            }
        }

        private void Shuffle(List<CardSO> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        public void DrawThreeCards(out IReadOnlyList<CardSO> currentCards)
        {
            if (_deckPool.Count == 0)
            {
                ReshuffleFromDiscard();
            }

            _currentDrawPool.Clear();

            int drawCount = Mathf.Min(3, _deckPool.Count);

            for (int i = 0; i < drawCount; i++)
            {
                CardSO card = _deckPool[0];
                _deckPool.RemoveAt(0);
                _currentDrawPool.Add(card);
            }
            currentCards = _currentDrawPool;
        }

        private void ReshuffleFromDiscard()
        {
            if (_discardPool.Count == 0)
                return;

            _deckPool.AddRange(_discardPool);
            _discardPool.Clear();
            Shuffle(_deckPool);
        }

        public void SelectCardById(string cardId)
        {
            CardSO cardSO = _currentDrawPool.FirstOrDefault(c => c.CardId.Equals(cardId));
            if (cardSO == null)
            {
                Debug.LogError("CardSO with id is not in the current draw");
                return;
            }
            SelectCard(cardSO);
        }

        public void SelectCard(CardSO card)
        {
            _selectedCard = card;
        }
        public void DeselectCurrentCard()
        {
            _selectedCard = null;
        }
    }
}