namespace SEP490G69
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameUIButton : Button
    {
        [Header("SFX")]
        [SerializeField] private string m_SFXId;

        [Header("Scale duration")]
        [SerializeField] private float m_ScaleDuration = 0.1f;
        [SerializeField] private Vector2 m_NormalScale = Vector2.one;
        [SerializeField] private Vector2 m_HighlightedScale = new Vector2(1.05f, 1.05f);
        [SerializeField] private Vector2 m_PressedScale = new Vector2(0.95f, 0.95f);
        [SerializeField] private Vector2 m_SelectedScale = Vector2.one;
        [SerializeField] private Vector2 m_DisabledScale = Vector2.one;

        [Header("Text color")]
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Color m_NormalColor;
        [SerializeField] private Color m_HighlightedColor;
        [SerializeField] private Color m_PressedColor;
        [SerializeField] private Color m_SelectedColor;
        [SerializeField] private Color m_DisabledColor;

        private AudioManager _audioManager;
        private AudioManager AudioManager => _audioManager ??= ContextManager.Singleton.ResolveGameContext<AudioManager>();

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
                    if (m_Text != null) m_Text.color = m_NormalColor;
                    break;
                case SelectionState.Highlighted:
                    transform.DOScale(m_HighlightedScale, m_ScaleDuration);
                    if (m_Text != null) m_Text.color = m_HighlightedColor;
                    break;
                case SelectionState.Pressed:
                    transform.DOScale(m_PressedScale, m_ScaleDuration);
                    if (m_Text != null) m_Text.color = m_PressedColor;
                    break;
                case SelectionState.Selected:
                    transform.DOScale(m_SelectedScale, m_ScaleDuration);
                    if (m_Text != null) m_Text.color = m_SelectedColor;
                    break;
                case SelectionState.Disabled:
                    transform.DOScale(m_DisabledScale, m_ScaleDuration);
                    if (m_Text != null) m_Text.color = m_DisabledColor;
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