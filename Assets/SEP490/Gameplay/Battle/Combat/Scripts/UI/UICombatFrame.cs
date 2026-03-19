namespace SEP490G69.Battle.Combat
{
    using DG.Tweening;
    using SEP490G69.Battle.Cards;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICombatFrame : BaseCombatFrame
    {
        [SerializeField] private UICharacterBaseDetails m_PlayerCharDetails;
        [SerializeField] private UICharacterBaseDetails m_EnemyCharDetails;
        [SerializeField] private Transform m_SelectedCardContainer;
        [SerializeField] private Button m_SettingBtn;
        [SerializeField] private Button m_RestBtn;
        [SerializeField] private Button m_ActionBtn;
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

        [SerializeField] private Transform m_EnemyCardActiveDisplayPoint;
        [SerializeField] private Transform m_EnemyCardActiveSpawnPoint;

        private Transform _enemySelectedCardTrans;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_SettingBtn.onClick.AddListener(ShowSettings);
            m_RestBtn.onClick.AddListener(PerformRest);
            m_ActionBtn.onClick.AddListener(PerformSelectCard);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_SettingBtn.onClick.RemoveListener(ShowSettings);
            m_RestBtn.onClick.RemoveListener(PerformRest);
            m_ActionBtn.onClick.RemoveListener(PerformSelectCard);
        }

        private void PerformRest()
        {
            CombatController.Player.SelectRest();
            CombatController.TurnSystem.ExecutePlayerCard();
        }

        private void PerformSelectCard()
        {
            CombatController.TurnSystem.ExecutePlayerCard();
        }

        private void ShowSettings()
        {

        }

        public UICombatFrame SetPlayerCharContent(string id, Sprite avatar)
        {
            m_PlayerCharDetails.SetContent(id, avatar);
            return this;
        }
        public UICombatFrame SetPlayerCharVit(float cur, float max)
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
        public UICombatFrame SetEnemyCharVit(float cur, float max)
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
                    effectUI.SetId(isPlayer ? "player" : "enemy").SetImg(effect.Data.Icon).SetRemainAmount(effect.Stack).SetOnClickCallback(SelectStatEffect);
                }
            }
        }

        /// <summary>
        /// Display cards by spawning card UI.
        /// Cards are spawned at the m_UISpawnPoint and move smoothly to m_CardContainer.
        /// In m_CardContainer, there's a Horizontal Layout Group Component.
        /// </summary>
        /// <param name="cards"></param>
        public void DisplayDrawnCards(IReadOnlyList<CardSO> cards)
        {
            Debug.Log("DisplayDrawnCards");
            ClearAllCards();
            StartCoroutine(CoDisplayCards(cards));
        }

        public void SpawnEnemyCard(CardSO card)
        {
            if (_enemySelectedCardTrans != null)
            {
                PoolManager.Pools[GameConstants.POOL_UI_CARD].DespawnObject(_enemySelectedCardTrans);
            }

            Transform spawnPoint = m_EnemyCardActiveSpawnPoint;
            Transform targetPoint = m_EnemyCardActiveDisplayPoint;

            Vector3 targetPosition = targetPoint.position;

            _enemySelectedCardTrans = PoolManager.Pools[GameConstants.POOL_UI_CARD].Spawn(m_CardPrefab, spawnPoint);
            RectTransform rect = _enemySelectedCardTrans.GetComponent<RectTransform>();
            LayoutElement layout = rect.GetComponent<LayoutElement>();
            rect.DOKill();

            UICardElement cardUI = _enemySelectedCardTrans.GetComponent<UICardElement>();
            if (cardUI != null)
            {
                string cardName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, card.CardName);
                string cardDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, card.CardDescription);

                cardUI.SetContent(card.CardId, cardName, cardDesc, card.Icon);
            }

            rect.position = spawnPoint.position;
            rect.DOMove(targetPosition, 0.55f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        if (layout != null)
                            layout.ignoreLayout = false;

                        StartCoroutine(DelayDespawn());
                    });
        }

        private IEnumerator DelayDespawn()
        {
            yield return new WaitForSeconds(0.2f);
            if (_enemySelectedCardTrans != null)
            {
                PoolManager.Pools[GameConstants.POOL_UI_CARD].DespawnObject(_enemySelectedCardTrans);
                _enemySelectedCardTrans = null;
            }
        }

        private IEnumerator CoDisplayCards(IReadOnlyList<CardSO> cards)
        {
            if (cards.Count == 0)
            {
                Debug.Log("No card of player");
                yield return null; ;
            }
            //m_CardContainer.GetComponent<HorizontalLayoutGroup>().enabled = false;
            for (int i = 0; i < cards.Count; i++)
            {
                CardSO card = cards[i];
                Debug.Log($"<color=green>[UICombatFrame.CoDisplayCards]</color> Card id: {card.CardId} - Name: {card.CardName}");
                Transform slotTrans = m_CardSlots[i];

                Transform cardTrans = PoolManager.Pools[GameConstants.POOL_UI_CARD].Spawn(m_CardPrefab, m_CardContainer);
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
                    string cardDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, card.CardDescription);

                    cardUI.SetOnSelectCallback(SelectCard)
                          .SetOnDragEnd(PerformCardAction)
                          .SetContent(card.CardId, cardName, cardDesc, card.Icon)
                          .SetCost(card.Cost);

                    cardUI._onDragParent = m_DraggingArea;
                }



                // Offset spawn so animation visible
                rect.position = m_UISpawnPoint.position;

                rect.DOMove(targetPos, 0.35f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        if (layout != null)
                            layout.ignoreLayout = false;
                    });

                rect.DOScale(1f, 0.35f);

                yield return new WaitForSeconds(0.05f); // card draw delay
            }
            //m_CardContainer.GetComponent<HorizontalLayoutGroup>().enabled = true;
        }

        public void ClearAllCards()
        {
            Debug.Log("ClearAllCards");

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

        private void PerformCardAction(string rawCardId, Transform currentParent)
        {
            UIDropHandler handler = currentParent.GetComponent<UIDropHandler>();
            if (handler != null)
            {
                if (handler.HandlerName.Equals(m_CardTriggerArea.HandlerName))
                {
                    CombatController.Player.SelectCardById(rawCardId);
                    CombatController.TurnSystem.ExecutePlayerCard();
                }
            }
        }

        private void SelectStatEffect(string characterOwner)
        {
            if (characterOwner.Equals("player"))
            {
                UIManager.ShowFrame(GameConstants.FRAME_ID_STAT_EFFECT_DETAILS)
                         .AsFrame<UIStatusEffectListFrame>()
                         .LoadStatusEffects(CombatController.Player.StatEffectManager.ActiveStatEffects);
            }
            else
            {
                UIManager.ShowFrame(GameConstants.FRAME_ID_STAT_EFFECT_DETAILS)
                         .AsFrame<UIStatusEffectListFrame>()
                         .LoadStatusEffects(CombatController.Player.StatEffectManager.ActiveStatEffects);
            }
        }
    }
}