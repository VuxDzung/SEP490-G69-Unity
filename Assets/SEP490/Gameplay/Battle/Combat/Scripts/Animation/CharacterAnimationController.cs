namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;

    /// <summary>
    /// Handle the Darkest dungeon animation combat logic.
    /// </summary>
    public class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private AnimationConfigSO m_Config;

        [SerializeField] private Transform m_PoseContainer;

        [SerializeField] private List<Transform> m_Poses;

        [Header("Testing")]
        [SerializeField] private string m_TestAnimation;


        private AnimationData _animationData;

        private Vector2 _defaultPosition;
        private float _startTime;
        private Transform _poseTrans;

        private Transform _combatPos;

        public void SetCombatPosition(Transform poseTrans)
        {
            _combatPos = poseTrans;
        }

        private void Awake()
        {
            LoadPoseTransforms();
        }

        private void OnEnable()
        {
            _defaultPosition = transform.position;
            _animationData = null;
            m_Animator.enabled = true;
        }
        private void OnDisable()
        {
            _animationData = null;
        }

        public void PlayAnimation()
        {
            PlayAnimation(m_TestAnimation);
        }

        private void LoadPoseTransforms()
        {
            m_Poses.Clear();
            for (int i = 0; i < m_PoseContainer.childCount; i++)
            {
                m_Poses.Add(m_PoseContainer.GetChild(i));
            }
        }

        public void PlayAnimation(string animName)
        {
            if (m_Config == null)
            {
                Debug.LogError($"[CharacterAnimationController] AnimationConfig of {gameObject.name} is null!");
                return;
            }

            AnimationData data = m_Config.GetAnimationByName(animName);

            if (data == null)
            {
                return;
            }

            _animationData = data;
            _poseTrans = m_Poses.FirstOrDefault(p => p.gameObject.name.Contains(_animationData.AnimationName));
            // disable animator
            m_Animator.enabled = false;
            _poseTrans.gameObject.SetActive(true);
            m_SpriteRenderer.enabled = false;

            // set pose sprite
            //if (data.PoseSprite != null)
            //{
            //    m_SpriteRenderer.sprite = data.PoseSprite;
            //}
            transform.position = _combatPos.position;

            // start timer
            _startTime = Time.time;
        }

        private void Update()
        {
            if (_animationData == null)
            {
                return;
            }

            float elapsed = Time.time - _startTime;
            float duration = _animationData.Duration;

            float t = Mathf.Clamp01(elapsed / _animationData.Duration);
            float speedMultiplier = _animationData.Curve.Evaluate(t);

            if (_animationData.Velocity.sqrMagnitude > 0f)
            {
                Vector3 move = (Vector3)(_animationData.Velocity * speedMultiplier * Time.deltaTime);
                transform.position += move;
            }

            if (elapsed >= duration)
            {
                transform.position = _defaultPosition;

                m_Animator.enabled = true;

                _animationData = null;

                m_SpriteRenderer.enabled = true;
                _poseTrans?.gameObject.SetActive(false);
                CombatCameraController.Singleton.ZoomCamera(false);
            }
        }
    }
}