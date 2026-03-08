namespace SEP490G69.Battle.Cards
{
    using SEP490G69.Addons.LoadScreenSystem;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIEditDeckFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private TextMeshProUGUI m_InDeckCountTmp;
        [SerializeField] private Button m_SaveDeckBtn;
        [SerializeField] private Transform m_InDeckContainer;

        [SerializeField] private TMP_InputField m_SearchInputTmp;
        [SerializeField] private Transform m_CardUIPrefab;

        private GameDeckController _deckController;
        protected GameDeckController DeckController
        {
            get
            {
                if (_deckController == null)
                {
                    _deckController = ContextManager.Singleton.ResolveGameContext<GameDeckController>();
                }
                return _deckController;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BackBtn.onClick.AddListener(Back);
            LoadObtainedCards();
            LoadInDeckCards();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackBtn.onClick.RemoveListener(Back);
        }

        /// <summary>
        /// Load in-deck cards.
        /// </summary>
        private void LoadInDeckCards()
        {

        }

        /// <summary>
        /// Load obtained cards here.
        /// Display by using PoolManager.Pools["UICard"].Spawn(prefab:Transform, container:Transform) to spawn card UI.
        /// </summary>
        private void LoadObtainedCards()
        {

        }

        private void Back()
        {
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
        }
    }
}