namespace SEP490G69
{
    using DG.Tweening;
    using System;
    using UnityEngine;

    public class FadingController : GlobalSingleton<FadingController>
    {
        [SerializeField] private CanvasGroup m_FadeGroup;

        protected override void CreateNewInstance()
        {
            base.CreateNewInstance();
            m_FadeGroup.gameObject.SetActive(false);
        }

        public void FadeIn(float fadeDuration)
        {
            m_FadeGroup.alpha = 0f;
            m_FadeGroup.gameObject.SetActive(true);
            m_FadeGroup.DOFade(1f, fadeDuration).onComplete = () => {
                m_FadeGroup.gameObject.SetActive(false);
            };
        }
        public void FadeOut(float fadeDuration)
        {
            m_FadeGroup.alpha = 1f;
            m_FadeGroup.gameObject.SetActive(true);
            m_FadeGroup.DOFade(0f, fadeDuration).onComplete = () => {
                m_FadeGroup.gameObject.SetActive(false);
            };
        }

        public void FadeIn2Out(float fadeDuration, float inFadeTime, Action onFadeInCompleted = null, Action onFadeOutCompleted = null)
        {
            m_FadeGroup.alpha = 0f;
            m_FadeGroup.gameObject.SetActive(true);

            m_FadeGroup.DOFade(1f, fadeDuration).onComplete = () => { onFadeInCompleted?.Invoke(); };

            m_FadeGroup.DOFade(0f, fadeDuration).SetDelay(inFadeTime).onComplete = () => {
                onFadeOutCompleted.Invoke();
                m_FadeGroup.gameObject.SetActive(false);
            };
        }
    }
}