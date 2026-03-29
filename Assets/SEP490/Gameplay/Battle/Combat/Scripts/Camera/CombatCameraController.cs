namespace SEP490G69.Battle.Combat
{
    using System.Collections;
    using UnityEngine;

    public class CombatCameraController : GlobalSingleton<CombatCameraController>
    {
        [SerializeField] private Camera m_Camera;
        [SerializeField] private CombatCameraConfigSO m_Config;
        [SerializeField] private float m_InCombatSize = 4f;
        [SerializeField] private float m_DefaultSize = 6f;

        [SerializeField] private Vector3 m_CombatShake = new Vector3(0.15f, 0.15f, 0f);
        [SerializeField] private float m_ShakeDuration = 0.15f;

        [SerializeField] private float m_ZoomSpeed = 5f;

        private Vector3 _defaultPos;

        private Coroutine _shakeRoutine;
        private Coroutine _zoomRoutine;

        protected override void Awake()
        {
            base.Awake();
            _defaultPos = m_Camera.transform.localPosition;
        }

        public void ShakeCamera()
        {
            if (_shakeRoutine != null)
                StopCoroutine(_shakeRoutine);

            _shakeRoutine = StartCoroutine(ShakeRoutine());
        }

        public void ZoomCamera(bool inCombat = true)
        {
            if (_zoomRoutine != null)
                StopCoroutine(_zoomRoutine);

            float target = inCombat ? m_Config.InCombatSize : m_Config.DefaultSize;
            _zoomRoutine = StartCoroutine(ZoomRoutine(target));
        }

        private IEnumerator ShakeRoutine()
        {
            float timer = 0f;

            while (timer < m_Config.ShakeDuration)
            {
                timer += Time.deltaTime;

                float x = Random.Range(-m_Config.CombatShake.x, m_Config.CombatShake.x);
                float y = Random.Range(-m_Config.CombatShake.y, m_Config.CombatShake.y);

                m_Camera.transform.localPosition = _defaultPos + new Vector3(x, y, 0);

                yield return null;
            }

            m_Camera.transform.localPosition = _defaultPos;
            _shakeRoutine = null;
        }

        private IEnumerator ZoomRoutine(float targetSize)
        {
            while (Mathf.Abs(m_Camera.orthographicSize - targetSize) > 0.01f)
            {
                m_Camera.orthographicSize = Mathf.Lerp(
                    m_Camera.orthographicSize,
                    targetSize,
                    Time.deltaTime * m_Config.ZoomSpeed
                );

                yield return null;
            }

            m_Camera.orthographicSize = targetSize;
            _zoomRoutine = null;
        }
    }
}