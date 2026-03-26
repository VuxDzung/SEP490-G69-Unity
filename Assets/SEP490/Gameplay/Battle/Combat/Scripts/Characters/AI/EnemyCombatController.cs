namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Collections;
    using System;
    using SEP490G69.Tournament;
    using System.Linq;

    //[RequireComponent(typeof(RandomSelectCardStrategy))]
    public class EnemyCombatController : BaseBattleCharacterController
    {
        [SerializeField] private float m_DelayPerfomAction = 1f;

        private ISelectCardStrategy _selectionStrategy;

        protected override void Awake()
        {
            base.Awake();
            _selectionStrategy = new CardContextWeightStrategy(this);
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


            SessionCharacterData readonlyCharData = _characterData.Clone() as SessionCharacterData;

            CharacterDataHolder runtimeDataHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(_characterData)
                                                      .Build();

            CharacterDataHolder readonlyDataHolder = new CharacterDataHolder.Builder()
                                                      .WithCharacterSO(characterSO)
                                                      .WithCharacterData(readonlyCharData).Build();

            SetReadonlyDataHolder(readonlyDataHolder);
            SetCharacterDataHolder(runtimeDataHolder);
            InitializeEnergySystem();

            EnemySO enemySO = characterSO.ConvertAs<EnemySO>();

            if (enemySO != null)
            {
                CombatCardsProcessor.InitializeDeck(enemySO.Deck.ToArray());
            }
            else
            {
                Debug.LogError("EnemySO is null");
            }
        }

        /// <summary>
        /// All of enemy brain are handled here.
        /// Step 1: Draw 3 random cards.
        /// Step 2: Pick a card (Randomly or use specific logic)
        /// Step 3
        /// </summary>
        public void DetermineCards(BaseBattleCharacterController opponent, Action<string> onCardSelected)
        {
            StartTurn();

            CombatCardsProcessor.DrawThreeCards(out IReadOnlyList<CardSO> cards);

            if (_selectionStrategy == null)
            {
                Debug.Log("Failed to fetch card selection strategy!");
                return;
            }

            if (_selectionStrategy.TrySelectCard(this, cards, out CardSO card))
            {
                if (CombatCardsProcessor.CalculateCardCost(card) > GetCombatStatus(EStatusType.Stamina).Value)
                {
                    onCardSelected?.Invoke("REST");
                    CombatCardsProcessor.SelectRest();
                }
                else
                {
                    onCardSelected?.Invoke(card.CardId);
                    CombatCardsProcessor.SelectCard(card);
                }
                StartCoroutine(DelayExecute(opponent));
            }
            else
            {
                Debug.LogError("Failed to select card. Choose rest as default!");
                onCardSelected?.Invoke("REST");
                CombatCardsProcessor.SelectRest();
                StartCoroutine(DelayExecute(opponent));
            }
        }

        private IEnumerator DelayExecute(BaseBattleCharacterController opponent)
        {
            yield return new WaitForSeconds(m_DelayPerfomAction);
            ExecuteCard(opponent);
        }
    }
}