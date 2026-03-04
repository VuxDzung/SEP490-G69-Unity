namespace SEP490G69
{
    using DG.Tweening;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class FadingController : GlobalSingleton<FadingController>
    {
        [SerializeField] private CanvasGroup m_FadeGroup;
        [SerializeField] private Image m_Image;
        [SerializeField] private TextMeshProUGUI m_MessageTmp;

        private Tween _typingTween;

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
            FadeIn2Out(fadeDuration, inFadeTime, Color.black, "", onFadeInCompleted, onFadeOutCompleted);
        }
        public void FadeIn2Out(float fadeDuration, float inFadeTime, string msg, Action onFadeInCompleted = null, Action onFadeOutCompleted = null)
        {
            FadeIn2Out(fadeDuration, inFadeTime, Color.black, msg, onFadeInCompleted, onFadeOutCompleted);
        }
        public void FadeIn2Out(float fadeDuration, float inFadeTime, Color fadeColor, string message, Action onFadeInCompleted = null, Action onFadeOutCompleted = null)
        {
            m_FadeGroup.alpha = 0f;
            m_FadeGroup.gameObject.SetActive(true);
            m_Image.color = fadeColor;
            m_FadeGroup.DOFade(1f, fadeDuration).onComplete = () => { 
                onFadeInCompleted?.Invoke();

                ShowWordByWordMsg(message, () =>
                {
                    m_FadeGroup.DOFade(0f, fadeDuration).SetDelay(inFadeTime).onComplete = () => {
                        m_FadeGroup.gameObject.SetActive(false);
                        onFadeOutCompleted?.Invoke();
                    };
                });
            };
        }

        /// <summary>
        /// Play a typing effect and display message.
        /// NOTE: This feature only works in Fade in to out method.
        /// </summary>
        private void ShowWordByWordMsg(string msg, Action onCompleted)
        {
            _typingTween?.Kill();
            _typingTween = null;

            if (m_MessageTmp == null)
            {
                onCompleted?.Invoke();
                return;
            }

            if (string.IsNullOrEmpty(msg))
            {
                m_MessageTmp.text = string.Empty;
                m_MessageTmp.gameObject.SetActive(false);
                onCompleted?.Invoke();
                return;
            }

            m_MessageTmp.gameObject.SetActive(true);
            m_MessageTmp.text = string.Empty;

            float typingDuration = Mathf.Max(0.5f, msg.Length * 0.05f);
            int currentIndex = 0;

            _typingTween = DOTween.To(() => currentIndex,
                                       x =>
                                       {
                                           currentIndex = x;
                                           m_MessageTmp.text = msg.Substring(0, currentIndex);
                                       },
                                       msg.Length,
                                       typingDuration)
                                  .SetEase(Ease.Linear)
                                  .SetUpdate(true).OnComplete(() => { onCompleted?.Invoke(); }); // Dung: run when timeScale = 0.
        }
    }
}