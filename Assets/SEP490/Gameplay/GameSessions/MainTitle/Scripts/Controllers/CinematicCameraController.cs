namespace SEP490G69
{
    using DG.Tweening;
    using UnityEngine;

    public class CinematicCameraController : MonoBehaviour
    {
        private static CinematicCameraController _instance;
        public static CinematicCameraController Instance => _instance;

        [SerializeField] private Camera m_Camera;

        private Tween _currentTween;
        private bool _isPlaying;

        public bool IsPlaying => _isPlaying;

        //-----------------------------------------
        // UNITY
        //-----------------------------------------
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        //private void Update()
        //{
        //    if (!_isZooming)
        //        return;

        //    _elapsedTime += Time.deltaTime;

        //    float t = Mathf.Clamp01(_elapsedTime / _duration);

        //    // Cinematic easing (smooth in-out)
        //    float easedT = EaseInOut(t);

        //    float newSize = Mathf.Lerp(_startSize, _targetSize, easedT);
        //    SetOrthSize(newSize);

        //    if (t >= 1f)
        //    {
        //        _isZooming = false;
        //    }
        //}

        //-----------------------------------------
        // PUBLIC API
        //-----------------------------------------
        //public void StartZoomIn(float size, float duration)
        //{
        //    if (m_Camera == null)
        //    {
        //        Debug.LogWarning("Camera not assigned!");
        //        return;
        //    }

        //    _startSize = m_Camera.orthographicSize;
        //    _targetSize = size;
        //    _duration = Mathf.Max(0.01f, duration);
        //    _elapsedTime = 0f;

        //    _isZooming = true;
        //}

        public void SetOrthSize(float size)
        {
            m_Camera.orthographicSize = size;
        }

        //-----------------------------------------
        // EASING
        //-----------------------------------------
        //private float EaseInOut(float t)
        //{
        //    // SmoothStep cinematic feel
        //    return t * t * (3f - 2f * t);
        //}

        public Tween DOZoom(float targetSize, float duration, System.Action onComplete)
        {
            if (m_Camera == null)
            {
                Debug.LogWarning("Camera not assigned!");
                onComplete?.Invoke();
                return null;
            }

            _currentTween?.Kill();
            _isPlaying = true;

            _currentTween = DOTween.To(
                    () => m_Camera.orthographicSize,
                    x => m_Camera.orthographicSize = x,
                    targetSize,
                    duration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    _currentTween = null;
                    onComplete?.Invoke();
                    _isPlaying = false;
                });

            return _currentTween;
        }

        public Tween DOMove(Vector3 direction, float duration, System.Action onComplete)
        {
            _currentTween?.Kill();

            Vector3 target = transform.position + direction;
            _isPlaying = true;
            _currentTween = transform.DOMove(target, duration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    _currentTween = null;
                    onComplete?.Invoke();
                    _isPlaying = false;
                });

            return _currentTween;
        }

        public void SetCameraXPosition(float x)
        {
            m_Camera.transform.position = new Vector3(x, m_Camera.transform.position.y, m_Camera.transform.position.z);
        }

        public float GetSize()
        {
            return m_Camera.orthographicSize;
        }
    }
}