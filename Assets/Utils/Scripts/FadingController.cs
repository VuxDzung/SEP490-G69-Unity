namespace SEP490G69
{
    using DG.Tweening;
    using System;
    using System.Collections.Generic;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.UI;

    public class PendingFadeSettings
    {
        public float Duration { get; set; }
        public float InFadeTime { get; set; }
        public Color FadeColor { get; set; }
        public string Message { get; set; }
        public Action onFadeInCompleted { get; set; }
        public Action onFadeOutCompleted { get;set; }
    }

    public class FadingController : GlobalSingleton<FadingController>
    {
        [SerializeField] private CanvasGroup m_FadeGroup;
        [SerializeField] private Image m_Image;
        [SerializeField] private TextMeshProUGUI m_MessageTmp;

        private Tween _typingTween;
        private readonly Queue<PendingFadeSettings> _pendingFadeInToOutQueue = new Queue<PendingFadeSettings>();
        private readonly Queue<PendingFadeSettings> _pendingFadeInQueue = new Queue<PendingFadeSettings>();
        private readonly Queue<PendingFadeSettings> _pendingFadeOutQueue = new Queue<PendingFadeSettings>();

        private bool _isFadeInToOut;
        private bool _isFadeIn;
        private bool _isFadeOut;

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
            _pendingFadeInQueue.Enqueue(new PendingFadeSettings
            {
                Duration = fadeDuration,
                FadeColor = fadeColor,
                onFadeInCompleted = onCompleted
            });
            TryFadeInNext();
        }

        private void TryFadeInNext()
        {
            if (_isFadeIn == true || _pendingFadeInQueue.Count == 0)
            {
                return;
            }

            var data = _pendingFadeInQueue.Dequeue();
            _isFadeIn = true;
            PlayFadeCinematic(data, 0, () =>
            {
                _isFadeIn = false;
                TryFadeInNext();
            });
        }

        private void PlayFadeCinematic(PendingFadeSettings data, float endValue, Action onCompleted)
        {
            m_FadeGroup.alpha = endValue == 0f ? 1f : 0f;
            m_Image.color = data.FadeColor;
            m_FadeGroup.gameObject.SetActive(true);
            m_FadeGroup.DOFade(endValue, data.Duration).onComplete = () => {
                data.onFadeInCompleted?.Invoke();
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
            _pendingFadeOutQueue.Enqueue(new PendingFadeSettings
            {
                Duration = fadeDuration,
                FadeColor = fadeColor,
                onFadeInCompleted = onCompleted
            });
            TryFadeOutNext();
            m_FadeGroup.alpha = 1f;
            m_Image.color = fadeColor;
            m_FadeGroup.gameObject.SetActive(true);
            m_FadeGroup.DOFade(0f, fadeDuration).onComplete = () => {
                onCompleted?.Invoke();
                m_FadeGroup.gameObject.SetActive(false);
            };
        }

        private void TryFadeOutNext()
        {
            if (_isFadeOut == true || _pendingFadeOutQueue.Count == 0)
            {
                return;
            }

            var data = _pendingFadeOutQueue.Dequeue();
            _isFadeOut = true;
            PlayFadeCinematic(data, 1f, () =>
            {
                _isFadeOut = false;
                TryFadeOutNext();
            });
        }

        public void FadeIn2Out(float fadeDuration, float inFadeTime, Action onFadeInCompleted = null, Action onFadeOutCompleted = null, bool needEnqueue = false)
        {
            FadeIn2Out(fadeDuration, inFadeTime, Color.black, "", onFadeInCompleted, onFadeOutCompleted, needEnqueue);
        }

        public void FadeIn2Out(float fadeDuration, float inFadeTime, string msg, Action onFadeInCompleted = null, Action onFadeOutCompleted = null, bool needEnqueue = false)
        {
            FadeIn2Out(fadeDuration, inFadeTime, Color.black, msg, onFadeInCompleted, onFadeOutCompleted, needEnqueue);
        }

        public void FadeIn2Out(float fadeDuration, float inFadeTime, Color fadeColor, string message, Action onFadeInCompleted = null, Action onFadeOutCompleted = null, bool needEnqueue = false)
        {
            var data = new PendingFadeSettings
            {
                Duration = fadeDuration,
                InFadeTime = inFadeTime,
                FadeColor = fadeColor,
                Message = message,
                onFadeInCompleted = onFadeInCompleted,
                onFadeOutCompleted = onFadeOutCompleted
            };

            if (needEnqueue)
            {
                _pendingFadeInToOutQueue.Enqueue(data);
                TryFadeIn2OutNext();
            }
            else
            {
                PlayFadeIn2OutCinematic(data, () =>
                {
                    TryFadeIn2OutNext();
                });
            }
        }

        private void TryFadeIn2OutNext()
        {
            if (_isFadeInToOut == true ||  _pendingFadeInToOutQueue.Count == 0)
            {
                return;
            }
            var data = _pendingFadeInToOutQueue.Dequeue();
            _isFadeInToOut = true;

            PlayFadeIn2OutCinematic(data, () =>
            {
                _isFadeInToOut = false;
                TryFadeIn2OutNext();
            });
        }

        private void PlayFadeIn2OutCinematic(PendingFadeSettings data, Action onCompleted)
        {
            m_FadeGroup.alpha = 0f;
            if (m_MessageTmp != null) m_MessageTmp.text = string.Empty;

            m_FadeGroup.gameObject.SetActive(true);
            m_Image.color = data.FadeColor;

            m_FadeGroup.DOFade(1f, data.Duration).onComplete = () => {
                data.onFadeInCompleted?.Invoke();

                ShowWordByWordMsg(data.Message, () =>
                {
                    m_FadeGroup.DOFade(0f, data.Duration).SetDelay(data.InFadeTime).onComplete = () => {
                        m_FadeGroup.gameObject.SetActive(false);
                        data.onFadeOutCompleted?.Invoke();
                        onCompleted?.Invoke();
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