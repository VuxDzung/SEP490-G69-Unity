namespace SEP490G69
{
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameUIButton : Button
    {
        [SerializeField] private string m_SFXId;
        [SerializeField] private float m_ScaleDuration = 0.15f;
        [SerializeField] private Vector2 m_NormalScale = Vector2.one;
        [SerializeField] private Vector2 m_HighlightedScale = Vector2.one;
        [SerializeField] private Vector2 m_SelectedScale = Vector2.one;
        [SerializeField] private Vector2 m_DisabledScale = Vector2.one;

        private AudioManager _audioManager;
        private AudioManager AudioManager
        {
            get
            {
                if(_audioManager == null)
                {
                    _audioManager = ContextManager.Singleton.ResolveGameContext<AudioManager>();
                }
                return _audioManager;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            onClick.AddListener(PlaySFX);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            onClick.RemoveListener(PlaySFX);
        }
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            switch(state)
            {
                case SelectionState.Normal:
                    transform.DOScale(m_NormalScale, m_ScaleDuration);
                    break;
                case SelectionState.Highlighted:
                    transform.DOScale(m_HighlightedScale, m_ScaleDuration);
                    break;
                case SelectionState.Pressed:
                    transform.DOScale(m_HighlightedScale, m_ScaleDuration);
                    break;
                case SelectionState.Selected:
                    transform.DOScale(m_SelectedScale, m_ScaleDuration);
                    break;
                case SelectionState.Disabled:
                    transform.DOScale(m_DisabledScale, m_ScaleDuration);
                    break;
            }
        }

        private void PlaySFX()
        {
            if (string.IsNullOrEmpty(m_SFXId))
            {
                return;
            }
            AudioManager.PlaySFX(m_SFXId);
        }
    }
}