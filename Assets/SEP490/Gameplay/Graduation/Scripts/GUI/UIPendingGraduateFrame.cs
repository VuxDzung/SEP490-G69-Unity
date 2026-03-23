namespace SEP490G69.Graduation
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPendingGraduateFrame : GameUIFrame
    {
        [SerializeField] private Button m_GraduateBtn;

        private GameGraduationController _graduateController;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (_graduateController == null)
            {
                _graduateController = ContextManager.Singleton.ResolveGameContext<GameGraduationController>();
            }
            m_GraduateBtn.onClick.AddListener(Graduate);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_GraduateBtn.onClick.RemoveListener(Graduate);
        }

        private void Graduate()
        {
            _graduateController.Graduate();
        }
    }
}