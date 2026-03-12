namespace SEP490G69.Battle.Combat
{
    using DG.Tweening;
    using SEP490G69.Battle.Cards;
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

        public void DisplayDrawnCards(IReadOnlyList<CardSO> cards)
        {
            if (PoolManager.Pools[GameConstants.POOL_UI_CARD].Count > 0)
            {
                PoolManager.Pools[GameConstants.POOL_UI_CARD].DespawnAll();
            }

            foreach (CardSO card in cards)
            {
                Transform cardTrans = PoolManager.Pools[GameConstants.POOL_UI_CARD].Spawn(m_CardPrefab, m_CardContainer);
                UICardElement cardUI = cardTrans.GetComponent<UICardElement>();
                if (cardUI != null)
                {
                    string cardName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, card.CardName);
                    string cardDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, card.CardDescription);
                    cardTrans.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    cardUI.SetOnSelectCallback(SelectCard)
                          .SetOnDragEnd(PerformCardAction)
                          .SetContent(card.CardId, cardName, cardDesc, card.Icon);
                }
            }
        }

        public void ClearAllCards()
        {
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
                    layout.ignoreLayout = true;
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