namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Status_", menuName = OrganizationConstants.NAMESPACE + "/Status/Status Data")]
    public class StatusDataSO : ScriptableObject
    {
        [SerializeField] private EStatusType m_StatusType;
        [SerializeField] private float m_BaseValue;

        public EStatusType StatusType => m_StatusType;
        public float BaseValue => m_BaseValue;
    }
}