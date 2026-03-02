namespace SEP490G69
{
    using UnityEngine;

    public class CinematicCameraController : MonoBehaviour
    {
        private static CinematicCameraController _instance;
        public static CinematicCameraController Instance => _instance;

        [SerializeField] private Camera m_Camera;

        private bool _isZooming;

        private float _startSize;
        private float _targetSize;
        private float _duration;
        private float _elapsedTime;

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

        private void Update()
        {
            if (!_isZooming)
                return;

            _elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(_elapsedTime / _duration);

            // Cinematic easing (smooth in-out)
            float easedT = EaseInOut(t);

            float newSize = Mathf.Lerp(_startSize, _targetSize, easedT);
            SetOrthSize(newSize);

            if (t >= 1f)
            {
                _isZooming = false;
            }
        }

        //-----------------------------------------
        // PUBLIC API
        //-----------------------------------------
        public void StartZoomIn(float size, float duration)
        {
            if (m_Camera == null)
            {
                Debug.LogWarning("Camera not assigned!");
                return;
            }

            _startSize = m_Camera.orthographicSize;
            _targetSize = size;
            _duration = Mathf.Max(0.01f, duration);
            _elapsedTime = 0f;

            _isZooming = true;
        }

        public void SetOrthSize(float size)
        {
            m_Camera.orthographicSize = size;
        }

        //-----------------------------------------
        // EASING
        //-----------------------------------------
        private float EaseInOut(float t)
        {
            // SmoothStep cinematic feel
            return t * t * (3f - 2f * t);
        }
    }
}