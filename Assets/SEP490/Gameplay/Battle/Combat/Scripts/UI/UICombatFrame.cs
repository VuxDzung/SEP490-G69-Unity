namespace SEP490G69.Battle.Combat
{
    using DG.Tweening;
    using SEP490G69.Battle.Cards;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Purchasing;
    using UnityEngine.UI;

    public class UICombatFrame : BaseCombatFrame
    {
        [SerializeField] private UICharacterBaseDetails m_PlayerCharDetails;
        [SerializeField] private UICharacterBaseDetails m_EnemyCharDetails;
        [SerializeField] private Transform m_SelectedCardContainer;
        [SerializeField] private Button m_SettingBtn;
        [SerializeField] private Button m_RestBtn;
        [SerializeField] private Button m_ActionBtn;
        [SerializeField] private Button m_EndTurnBtn;
        [SerializeField] private Button m_DeckPoolBtn;
        [SerializeField] private Button m_DiscardPoolBtn;
        [SerializeField] private Transform m_CardPrefab;
        [SerializeField] private Transform m_CardContainer;
        [SerializeField] private Transform m_PlayerStatEffectContainer;
        [SerializeField] private Transform m_EnemyStatEffectContainer;
        [SerializeField] private Transform m_StatEffectUIPrefab;

        [SerializeField] private UIDropHandler m_CardTriggerArea;

        [SerializeField] private Transform m_UISpawnPoint;
        [SerializeField] private Transform m_UIDiscardPoint;
        [SerializeField] private Transform m_DraggingArea;

        [SerializeField] private Transform[] m_CardSlots;

        [Header("Auto-combat")]
        [SerializeField] private Transform m_PlayerCardActiveDisplayPoint;
        [SerializeField] private Transform m_PlayerCardActiveSpawnPoint;
        [SerializeField] private Transform m_EnemyCardActiveDisplayPoint;
        [SerializeField] private Transform m_EnemyCardActiveSpawnPoint;
        [SerializeField] private float m_CardMoveTime = 0.5f;
        [SerializeField] private float m_DelayCardDespawnTime = 0.3f;

        private Transform _enemySelectedCardTrans;
        private Transform _playerSelectedCardTrans;
        private SceneCombatController _sceneController;
        private SceneCombatController SceneController => _sceneController ??= ContextManager.Singleton.GetSceneContext<SceneCombatController>();

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_SettingBtn.onClick.AddListener(ShowSettings);
            m_RestBtn.onClick.AddListener(PerformRest);
            m_ActionBtn.onClick.AddListener(PerformSelectCard);
            m_EndTurnBtn.onClick.AddListener(EndPlayerTurn);
            m_DeckPoolBtn.onClick.AddListener(ShowDeckPool);
            m_DiscardPoolBtn.onClick.AddListener(ShowDiscardPool);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_SettingBtn.onClick.RemoveListener(ShowSettings);
            m_RestBtn.onClick.RemoveListener(PerformRest);
            m_ActionBtn.onClick.RemoveListener(PerformSelectCard);
            m_EndTurnBtn.onClick.RemoveListener(EndPlayerTurn);
            m_DeckPoolBtn.onClick.RemoveListener(ShowDeckPool);
            m_DiscardPoolBtn.onClick.RemoveListener(ShowDiscardPool);
        }

        private void PerformRest()
        {
            CombatController.Player.SelectRest();
            CombatController.TurnProcessor.ExecutePlayerCard();
        }

        private void PerformSelectCard()
        {
            CombatController.TurnProcessor.ExecutePlayerCard();
        }

        private void ShowSettings()
        {

        }

        public UICombatFrame SetPlayerCharContent(string id, Sprite avatar)
        {
            m_PlayerCharDetails.SetContent(id, avatar);
            return this;
        }
        public UICombatFrame SetPlayerCharHP(float cur, float max)
        {
            m_PlayerCharDetails.SetVit(cur, max);
            return this;
        }
        public UICombatFrame SetPlayerCharStamina(float cur, float max)
        {
            m_PlayerCharDetails.SetStamina(cur, max);
            return this;
        }
        public UICombatFrame SetPlayerCharGauge(float cur, float max)
        {
            m_PlayerCharDetails.SetSpeed(cur, max);
            return this;
        }

        public UICombatFrame SetEnemyCharContent(string id, Sprite avatar)
        {
            m_EnemyCharDetails.SetContent(id, avatar);
            return this;
        }
        public UICombatFrame SetEnemyCharHP(float cur, float max)
        {
            m_EnemyCharDetails.SetVit(cur, max);
            return this;
        }
        public UICombatFrame SetEnemyCharStamina(float cur, float max)
        {
            m_EnemyCharDetails.SetStamina(cur, max);
            return this;
        }
        public UICombatFrame SetEnemyCharGauge(float cur, float max)
        {
            m_EnemyCharDetails.SetSpeed(cur, max);
            return this;
        }

        public UICombatFrame LoadPlayerStatEffects(IReadOnlyList<RuntimeStatusEffect> effectList)
        {
            LoadStatEffects("UIPlayerStatusEffect", effectList, true);
            return this;
        }

        public UICombatFrame LoadEnemyStatEffects(IReadOnlyList<RuntimeStatusEffect> effectList)
        {
            LoadStatEffects("UIEnemyStatusEffect", effectList, false);
            return this;
        }

        private void EndPlayerTurn()
        {
            SceneController.ChangetoEnemyTurn();
        }

        private void LoadStatEffects(string poolName, IReadOnlyList<RuntimeStatusEffect> effectList, bool isPlayer)
        {
            if (PoolManager.Pools[poolName].Count > 0)
            {
                PoolManager.Pools[poolName].DespawnAll();
            }
            Transform container = isPlayer ? m_PlayerStatEffectContainer : m_EnemyStatEffectContainer;
            foreach (RuntimeStatusEffect effect in effectList)
            {
                Transform effectTrans = PoolManager.Pools[poolName].Spawn(m_StatEffectUIPrefab, container);
                UIStatusEffectElement effectUI = effectTrans.GetComponent<UIStatusEffectElement>();
                if (effectUI != null)
                {
                    effectUI.SetId(isPlayer ? "player" : "enemy")
                            .SetImg(effect.Data.Icon)
                            .SetRemainAmount(effect.Stack)
                            .SetStatusName(LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_STATUS_EFFECT_NAMES, effect.Data.EffectName))
                            .SetOnClickCallback(SelectStatEffect);
                }
            }
        }

        /// <summary>
        /// Display cards by spawning card UI.
        /// Cards are spawned at the m_UISpawnPoint and move smoothly to m_CardContainer.
        /// In m_CardContainer, there's a Horizontal Layout Group Component.
        /// </summary>
        /// <param name="cards"></param>
        public void DisplayDrawnCards(IReadOnlyList<CardSO> cards, ICombatCardsService cardsService, float stamina)
        {
            ClearAllCards();
            StartCoroutine(CoDisplayCards(cards, cardsService, stamina));
            m_RestBtn.gameObject.SetActive(true);
        }

        public void SpawnEnemyCard(CardSO card, ICombatCardsService cardsService)
        {
            SpawnSelectedCard(card, true, cardsService);
        }

        public void SpawnPlayerAutoCard(CardSO card, ICombatCardsService cardsService)
        {
            SpawnSelectedCard(card, false, cardsService);
        }

        private void ShowDeckPool()
        {
            ICombatCardsService cardsService = SceneController.Player.CardsService;

            UIManager.ShowFrame(GameConstants.FRAME_ID_COMBAT_POOL_CARD)
                     .AsFrame<UICombatCardPoolFrame>()
                     .LoadCards(cardsService.GetInDeckCards(), cardsService);
        }

        private void ShowDiscardPool()
        {
            ICombatCardsService cardsService = SceneController.Player.CardsService;

            UIManager.ShowFrame(GameConstants.FRAME_ID_COMBAT_POOL_CARD)
                     .AsFrame<UICombatCardPoolFrame>()
                     .LoadCards(cardsService.GetDiscardedCards(), cardsService);
        }

        private void SpawnSelectedCard(CardSO card, bool isEnemy, ICombatCardsService cardsService)
        {
            Transform selectedCardTrans = isEnemy ? _enemySelectedCardTrans : _playerSelectedCardTrans;

            if (selectedCardTrans != null)
            {
                PoolManager.Pools[GameConstants.POOL_UI_CARD].DespawnObject(selectedCardTrans);
                selectedCardTrans = null;
            }

            Transform spawnPoint = isEnemy ? m_EnemyCardActiveSpawnPoint : m_PlayerCardActiveSpawnPoint;
            Transform targetPoint = isEnemy ? m_EnemyCardActiveDisplayPoint : m_PlayerCardActiveDisplayPoint;

            Vector3 targetPosition = targetPoint.position;

            UICardElement cardUI = null;
            RectTransform rect = null;
            LayoutElement layout = null;

            Transform cardUITrans = null;

            if (isEnemy)
            {
                _enemySelectedCardTrans = PoolManager.Pools[GameConstants.POOL_UI_CARD].Spawn(m_CardPrefab, spawnPoint);
                cardUITrans = _enemySelectedCardTrans;
            }
            else
            {
                _playerSelectedCardTrans = PoolManager.Pools[GameConstants.POOL_UI_CARD].Spawn(m_CardPrefab, spawnPoint);
                cardUITrans = _playerSelectedCardTrans;
            }

            rect = cardUITrans.GetComponent<RectTransform>();
            layout = rect.GetComponent<LayoutElement>();
            rect.DOKill();
            cardUI = cardUITrans.GetComponent<UICardElement>();

            if (cardUI != null)
            {
                string cardName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, card.CardName);
                string cardDesc = cardsService.GetFinalCardDescription(card, LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, card.CardDescription));
                Debug.Log($"Card name: {cardName}");
                cardUI.SetContent(card.CardId, cardName, cardDesc, card.Icon);
            }
            else
            {
                Debug.Log("Card UI Element is null");
            }

            rect.position = spawnPoint.position;
            rect.DOMove(targetPosition, m_CardMoveTime)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        if (layout != null)
                        {
                            layout.ignoreLayout = false;
                        }

                        StartCoroutine(DelayDespawn());
                    });
        }

        private IEnumerator DelayDespawn()
        {
            yield return new WaitForSeconds(m_DelayCardDespawnTime);
            if (_enemySelectedCardTrans != null)
            {
                PoolManager.Pools[GameConstants.POOL_UI_CARD].DespawnObject(_enemySelectedCardTrans);
                _enemySelectedCardTrans = null;
            }
            if (_playerSelectedCardTrans != null)
            {
                PoolManager.Pools[GameConstants.POOL_UI_CARD].DespawnObject(_playerSelectedCardTrans);
                _playerSelectedCardTrans = null;
            }
        }

        private IEnumerator CoDisplayCards(IReadOnlyList<CardSO> cards, ICombatCardsService cardProcessor, float currentStamina)
        {
            if (cards.Count == 0)
            {
                Debug.Log("No card of player");
                yield return null; ;
            }

            for (int i = 0; i < cards.Count; i++)
            {
                CardSO card = cards[i];

                Transform slotTrans = m_CardSlots[i];

                Transform cardTrans = PoolManager.Pools[GameConstants.POOL_UI_CARD].Spawn(m_CardPrefab, slotTrans);
                RectTransform rect = cardTrans.GetComponent<RectTransform>();
                LayoutElement layout = rect.GetComponent<LayoutElement>();

                if (layout != null)
                {
                    layout.ignoreLayout = true;
                }
                // Target position (slot in layout)
                Vector3 targetPos = slotTrans.position;

                // Spawn at spawn point
                rect.position = m_UISpawnPoint.position;
                rect.localScale = Vector3.one * 0.7f;

                UICardElement cardUI = cardTrans.GetComponent<UICardElement>();

                if (cardUI != null)
                {
                    string cardName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, card.CardName);
                    string cardDesc = cardProcessor.GetFinalCardDescription(card, LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, card.CardDescription));
                    int cardCost = cardProcessor.CalculateCardCost(card);
                    cardUI.SetOnSelectCallback(SelectCard)
                          .SetOnDragEnd(PerformCardAction)
                          .SetContent(card.CardId, cardName, cardDesc, card.Icon)
                          .SetCost(cardCost);
                    cardUI._onDragParent = m_DraggingArea;

                    if (cardCost > currentStamina || SceneController.IsCardUsable(card) == false)
                    {
                        cardUI.IsDraggable = false;
                    }
                    else
                    {
                        cardUI.IsDraggable = true;
                    }
                }

                // Offset spawn so animation visible
                rect.position = m_UISpawnPoint.position;

                rect.DOMove(targetPos, 0.5f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        if (layout != null)
                        {
                            layout.ignoreLayout = false;
                        }
                        rect.localPosition = Vector3.zero;
                    });

                rect.DOScale(1f, 0.4f);

                yield return new WaitForSeconds(0.1f); // card draw delay
            }
        }

        public void ClearAllCards()
        {
            Debug.Log("ClearAllCards");
            m_RestBtn.gameObject.SetActive(false);

            if (PoolManager.Pools[GameConstants.POOL_UI_CARD].Count > 0)
            {
                PoolManager.Pools[GameConstants.POOL_UI_CARD].DespawnAll();
            }
        }

        private void SelectCard(string cardId, bool isSelected, Transform cardTrans)
        {
            if (CombatController == null)
            {
                Debug.LogError("CombatController is null");
                return;
            }

            RectTransform rect = cardTrans as RectTransform;
            LayoutElement layout = rect.GetComponent<LayoutElement>();
            rect.DOKill();

            if (isSelected)
            {
                CombatController.Player.SelectCardById(cardId);
                if (layout != null)
                {
                    layout.ignoreLayout = true;
                }
                //cardTrans.SetParent(m_SelectedCardContainer, false);

                //ResetRectTransform(rect, ERectPivot.MiddleCenter);

                rect.localScale = Vector3.one * 0.8f;
                //rect.DOLocalMove(Vector3.zero, 0.3f);
                rect.DOScale(1f, 0.3f);
            }
            else
            {
                //CombatController.Player.DeselectCurrentCard();

                //cardTrans.SetParent(m_CardContainer, false);
                if (layout != null)
                    layout.ignoreLayout = false;

                //ResetRectTransform(rect, ERectPivot.BottomRight);

                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
            }
        }

        private void PerformCardAction(string rawCardId, Transform currentParent, Transform draggableContent)
        {
            UIDropHandler handler = currentParent.GetComponent<UIDropHandler>();
            if (handler != null)
            {
                if (handler.HandlerName.Equals(m_CardTriggerArea.HandlerName))
                {
                    CombatController.Player.SelectCardById(rawCardId);
                    CombatController.TurnProcessor.ExecutePlayerCard();
                    PoolManager.Pools[GameConstants.POOL_UI_CARD].DespawnObject(draggableContent);
                }
            }
        }

        private void SelectStatEffect(string characterOwner)
        {
            if (characterOwner.Equals("player"))
            {
                UIManager.ShowFrame(GameConstants.FRAME_ID_STAT_EFFECT_DETAILS)
                         .AsFrame<UIStatusEffectListFrame>()
                         .LoadStatusEffects(CombatController.Player.EffectsManager.ActiveStatEffects);
            }
            else
            {
                UIManager.ShowFrame(GameConstants.FRAME_ID_STAT_EFFECT_DETAILS)
                         .AsFrame<UIStatusEffectListFrame>()
                         .LoadStatusEffects(CombatController.Player.EffectsManager.ActiveStatEffects);
            }
        }
    }
}