namespace SEP490G69.Graduation
{
    using System.Collections;
    using System.Collections.Generic;
    using SEP490G69.Addons.LoadScreenSystem;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPendingGraduateFrame : GameUIFrame
    {
        [SerializeField] private Button m_GraduateBtn;

        private GameGraduationController _graduateController;
        private GameGraduationController GraduateController
        {
            get
            {
                if (_graduateController == null)
                {
                    _graduateController = ContextManager.Singleton.ResolveGameContext<GameGraduationController>();
                }
                return _graduateController;
            }
        }
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
            FadingController.Singleton.FadeIn2Out(1f, 1f, () =>
            {
                List<LoadTask> postLoadTasks = new List<LoadTask>
            {
                new LoadTask("Graduating", DelayGraduation),
            };
                SceneLoader.Singleton.StartLoad(GameConstants.SCENE_GRADUATION, null, postLoadTasks);
            });
        }

        private IEnumerator DelayGraduation()
        {
            yield return new WaitForSeconds(0.5f);
            GraduateController.Graduate();
        }
    }
}