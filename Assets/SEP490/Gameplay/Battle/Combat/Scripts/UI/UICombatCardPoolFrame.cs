namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICombatCardPoolFrame : GameUIFrame
    {
        [SerializeField] private Transform m_CardUIPrefab;
        [SerializeField] private Transform m_CardContainer;
        [SerializeField] private Button m_CloseBtn;

        private ImageMasterConfigSO _imgMasterConfig;
        private ImageMasterConfigSO ImgMasterConfig => _imgMasterConfig ??= Resources.Load<ImageMasterConfigSO>("Images/ImageMasterConfig");

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_CloseBtn.onClick.AddListener(HideThisView);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            ClearAllContents();
            m_CloseBtn.onClick.RemoveListener(HideThisView);
        }

        private void ClearAllContents()
        {
            if (PoolManager.Pools["PoolCard"].Count > 0)
            {
                PoolManager.Pools["PoolCard"].DespawnAll();
            }
        }

        public void LoadCards(IReadOnlyList<CardSO> cards, ICombatCardsService cardsService)
        {
            ClearAllContents();

            foreach (var card in cards)
            {
                Transform cardUITrans = PoolManager.Pools["PoolCard"].Spawn(m_CardUIPrefab, m_CardContainer);
                UICardElement cardUI = cardUITrans.GetComponent<UICardElement>();
                if (cardUI != null)
                {
                    string cardName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, card.CardName);
                    string cardDesc = cardsService.GetFinalCardDescription(card, LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, card.CardDescription));
                    Sprite cardTypeSprite = GetCardTypeImg(card.ActionType);
                    int cardCost = cardsService.CalculateCardCost(card);
                    cardUI.SetContent(card.CardId, cardName, cardDesc, card.Icon)
                          .SetCost(cardCost);
                    cardUI.SetCardTypeSprite(cardTypeSprite);
                    cardUI.IsDraggable = false;
                }
            }
        }

        public Sprite GetCardTypeImg(EActionType type)
        {
            string id = CardConstants.GetCardTypeIconId(type);

            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return ImgMasterConfig.GetImage("card_types", id)?.image;
        }
    }
}