namespace SEP490G69
{
    using DG.Tweening;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class FadingController : GlobalSingleton<FadingController>
    {
        [SerializeField] private CanvasGroup m_FadeGroup;
        [SerializeField] private Image m_Image;

        protected override void CreateNewInstance()
        {
            base.CreateNewInstance();
            m_FadeGroup.gameObject.SetActive(false);
        }

        public void FadeIn(float fadeDuration, Action onCompleted = null)
        {
            FadeIn(fadeDuration, Color.black, onCompleted);
        }
        public void FadeIn(float fadeDuration, Color fadeColor, Action onCompleted = null)
        {
            m_FadeGroup.alpha = 0f;
            m_Image.color = fadeColor;
            m_FadeGroup.gameObject.SetActive(true);
            m_FadeGroup.DOFade(1f, fadeDuration).onComplete = () => {
                onCompleted?.Invoke();
                m_FadeGroup.gameObject.SetActive(false);
            };
        }

        public void FadeOut(float fadeDuration, Action onCompleted = null)
        {
            FadeOut(fadeDuration, Color.black, onCompleted);
        }

        public void FadeOut(float fadeDuration, Color fadeColor, Action onCompleted = null)
        {
            m_FadeGroup.alpha = 1f;
            m_Image.color = fadeColor;
            m_FadeGroup.gameObject.SetActive(true);
            m_FadeGroup.DOFade(0f, fadeDuration).onComplete = () => {
                onCompleted?.Invoke();
                m_FadeGroup.gameObject.SetActive(false);
            };
        }

        public void FadeIn2Out(float fadeDuration, float inFadeTime, Action onFadeInCompleted = null, Action onFadeOutCompleted = null)
        {
            FadeIn2Out(fadeDuration, inFadeTime, Color.black, onFadeInCompleted, onFadeOutCompleted);
        }
        public void FadeIn2Out(float fadeDuration, float inFadeTime, Color fadeColor, Action onFadeInCompleted = null, Action onFadeOutCompleted = null)
        {
            m_FadeGroup.alpha = 0f;
            m_FadeGroup.gameObject.SetActive(true);
            m_Image.color = fadeColor;
            m_FadeGroup.DOFade(1f, fadeDuration).onComplete = () => { onFadeInCompleted?.Invoke(); };

            m_FadeGroup.DOFade(0f, fadeDuration).SetDelay(inFadeTime).onComplete = () => {
                m_FadeGroup.gameObject.SetActive(false);
                onFadeOutCompleted?.Invoke();
            };
        }
    }
}