namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyCombatController : BaseBattleCharacterController
    {
        private List<ISelectCardStrategy> _cardSelectStrategies;

        private void Awake()
        {
            _cardSelectStrategies.Clear();
            _cardSelectStrategies.AddRange(GetComponents<ISelectCardStrategy>());
        }

        /// <summary>
        /// Because the enemy only exists in battle, the system must manually create CharacterDataHolder for the enemy by itself.
        /// </summary>
        /// <param name="characterId"></param>
        public override void Initialize(BaseCharacterSO characterSO)
        {
            SessionCharacterData _characterData = new SessionCharacterData();
            _characterData.Id = characterSO.CharacterId;
            _characterData.CurrentMaxVitality = characterSO.BaseVit;
            _characterData.CurrentPower = characterSO.BasePow;
            _characterData.CurrentIntelligence = characterSO.BaseInt;
            _characterData.CurrentStamina = characterSO.BaseSta;
            _characterData.CurrentDef = characterSO.BaseDef;
            _characterData.CurrentAgi = characterSO.BaseAgi;

            CharacterDataHolder runtimeDataHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(_characterData)
                                                      .Build();

            CharacterDataHolder readonlyDataHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(_characterData).Build();
            SetReadonlyDataHolder(readonlyDataHolder);
            SetCharacterDataHolder(runtimeDataHolder);
            InitializeEnergySystem();
        }

        /// <summary>
        /// All of enemy brain are handled here.
        /// Step 1: Draw 3 random cards.
        /// Step 2: Pick a card (Randomly or use specific logic)
        /// Step 3
        /// </summary>
        public void DetermineCards(BaseBattleCharacterController enemy)
        {
            OnTurnStart();

            DrawThreeCards(out IReadOnlyList<CardSO> cards);

            var strategy = GetFirstStrategy();

            if (strategy == null)
            {
                Debug.Log("Failed to fetch card selection strategy!");
                return;
            }

            if (strategy.TrySelectCard(ReadonlyDataHolder, CharacterDataHolder, cards, out CardSO card))
            {
                SelectCard(card);

                ReceiveCardEffect(this, enemy);

                StartNewTurn();
            }
            else
            {
                Debug.LogError("Failed to select card");
            }
        }

        public ISelectCardStrategy GetFirstStrategy()
        {
            if (_cardSelectStrategies.Count == 0)
            {
                return null;
            }
            return _cardSelectStrategies[0];
        }

        public ISelectCardStrategy GetStrategyByType<T>() where T : ISelectCardStrategy
        {
            foreach (var strategy in _cardSelectStrategies)
            {
                if (strategy is T _strategy)
                {
                    return _strategy;
                }
            }
            return null;
        }
    }
}