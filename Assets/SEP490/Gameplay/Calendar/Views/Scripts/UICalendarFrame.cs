namespace SEP490G69.Calendar
{
    using SEP490G69.Training;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICalendarFrame : GameUIFrame
    {
        [SerializeField] private Button m_PrevBtn;
        [SerializeField] private Button m_NextBtn;
        [SerializeField] private TextMeshProUGUI m_CurrentTimeTmp;
        [SerializeField] private Button m_BackBtn;

        private GameCalendarController _calendarController;
        private GameCalendarController CalendarController
        {
            get
            {
                if (_calendarController == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext(out _calendarController);
                }
                return _calendarController;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_PrevBtn.onClick.AddListener(Prev);
            m_NextBtn.onClick.AddListener(Next);
            m_BackBtn.onClick.AddListener(Back);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_PrevBtn.onClick.RemoveListener(Prev);
            m_NextBtn.onClick.RemoveListener(Next);
            m_BackBtn.onClick.RemoveListener(Back);
        }

        public void LoadCalendarTime()
        {
            m_CurrentTimeTmp.text = CalendarController.GetCalendarTime();
        }

        private void Prev()
        {

        }
        private void Next()
        {

        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
        }
    }
}