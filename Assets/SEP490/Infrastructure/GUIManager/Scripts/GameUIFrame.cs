namespace SEP490G69
{
    using SEP490G69.Addons.Localization;
    using UnityEngine;

    public class GameUIFrame : CoreBehaviour
    {
        [Tooltip("The id of the frame. Remember the id must be unique")]
        [SerializeField] private string frameId;

        /// <summary>
        /// The id of the frame.
        /// </summary>
        public string FrameId => frameId;

        /// <summary>
        /// Determine whether the frame is shown/hidden.
        /// </summary>
        public bool IsDisplayed { get; private set; }

        /// <summary>
        /// The manager's instance of the frame.
        /// </summary>
        protected GameUIManager UIManager { get; private set; }

        /// <summary>
        /// Set the SimpleUIManager reference for this frame.
        /// </summary>
        /// <param name="manager"></param>
        public void SetManager(GameUIManager manager)
        {
            this.UIManager = manager;
            IsDisplayed = false;
        }

        /// <summary>
        /// Show the frame. Called by UI Manager only.
        /// </summary>
        public void Show()
        {
            if (!IsDisplayed)
            {
                IsDisplayed = true;
                gameObject.SetActive(true);
                OnFrameShown();
            }
        }

        /// <summary>
        /// Show the frame. Called by UI Manager only.
        /// </summary>
        public void Hide()
        {
            if (IsDisplayed)
            {
                OnFrameHidden();
                IsDisplayed = false;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called after the frame is shown.
        /// </summary>
        protected virtual void OnFrameShown() { }

        /// <summary>
        /// Called before the frame is hidden
        /// </summary>
        protected virtual void OnFrameHidden() { }

        /// <summary>
        /// Convert the SimpleUIFrame to the T instance which inherit this class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AsFrame<T>() where T : GameUIFrame
        {
            return (T)this;
        }

        /// <summary>
        /// Hide this current frame. This is called by frame which inherits this base class.
        /// </summary>
        protected void HideThisView()
        {
            UIManager.HideFrame(frameId);
        }

        #region Lazy properties

        private LocalizationManager _localizeManager;
        protected LocalizationManager LocalizeManager
        {
            get
            {
                if (_localizeManager == null)
                {
                    _localizeManager = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();
                }
                return _localizeManager;
            }
        }

        private EventManager _eventManager;
        protected EventManager EventManager
        {
            get
            {
                if (_eventManager == null)
                {
                    _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
                }
                return _eventManager;
            }
        }
        #endregion
    }
}