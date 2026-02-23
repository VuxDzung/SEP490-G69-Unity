namespace SEP490G69.Training
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIMainMenuFrame : GameUIFrame
    {
        [SerializeField] private Button m_SettingsBtn;
        [SerializeField] private Button m_NoAdsBtn;
        [SerializeField] private Button m_HallOfFameBtn;
        [SerializeField] private Button m_ShopBtn;

        [Header("Actions")]
        [SerializeField] private Button m_TrainingBtn;
        [SerializeField] private Button m_RestBtn;
        [SerializeField] private Button m_CalendarBtn;
        [SerializeField] private Button m_DeckBtn;
        [SerializeField] private Button m_InventoryBtn;
        [SerializeField] private Button m_CardsBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
#if !UNITY_ANDROID
            m_NoAdsBtn.gameObject.SetActive(false);
#else
            m_NoAdsBtn.gameObject.SetActive(true);
#endif
            m_SettingsBtn.onClick.AddListener(ShowSettingsFrame);
            m_NoAdsBtn.onClick.AddListener(PurchaseNoAds);
            m_HallOfFameBtn.onClick.AddListener(ShowHallOfFame);
            m_ShopBtn.onClick.AddListener(ShowShop);

            m_TrainingBtn.onClick.AddListener(ShowTrainingMenu);
            m_RestBtn.onClick.AddListener(PerformRest);
            m_CalendarBtn.onClick.AddListener(ShowCalendar);
            m_DeckBtn.onClick.AddListener(ShowDeck);
            m_CardsBtn.onClick.AddListener(ShowCards);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_SettingsBtn.onClick.RemoveListener(ShowSettingsFrame);
            m_NoAdsBtn.onClick.RemoveListener(PurchaseNoAds);
            m_HallOfFameBtn.onClick.RemoveListener(ShowHallOfFame);
            m_ShopBtn.onClick.RemoveListener(ShowShop);

            m_TrainingBtn.onClick.RemoveListener(ShowTrainingMenu);
            m_RestBtn.onClick.RemoveListener(PerformRest);
            m_CalendarBtn.onClick.RemoveListener(ShowCalendar);
            m_DeckBtn.onClick.RemoveListener(ShowDeck);
            m_CardsBtn.onClick.RemoveListener(ShowCards);
        }

        private void ShowSettingsFrame()
        {

        }
        private void PurchaseNoAds()
        {

        }
        private void ShowHallOfFame()
        {

        }
        private void ShowShop()
        {

        }

        private void ShowTrainingMenu()
        {

        }

        private void PerformRest()
        {

        }

        private void ShowCalendar()
        {

        }
        private void ShowDeck()
        {

        }
        private void ShowCards()
        {

        }
    }
}