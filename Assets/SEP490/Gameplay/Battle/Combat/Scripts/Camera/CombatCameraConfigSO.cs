namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "CombatCameraConfig")]
    public class CombatCameraConfigSO : ScriptableObject
    {
        [SerializeField] private float m_InCombatSize = 4f;
        [SerializeField] private float m_DefaultSize = 6f;

        [SerializeField] private Vector3 m_CombatShake = new Vector3(0.15f, 0.15f, 0f);
        [SerializeField] private float m_ShakeDuration = 0.15f;

        [SerializeField] private float m_ZoomSpeed = 5f;

        public float InCombatSize { get => m_InCombatSize; }
        public float DefaultSize { get => m_DefaultSize;}
        public Vector3 CombatShake { get => m_CombatShake; }
        public float ShakeDuration { get => m_ShakeDuration; }
        public float ZoomSpeed { get => m_ZoomSpeed; }
    }
}