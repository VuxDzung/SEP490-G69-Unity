namespace SEP490G69
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EventNode_", menuName = OrganizationConstants.NAMESPACE + "/Dialog Framework/Dialog Event Node")]
    public class EventNodeSO : BaseDialogNodeSO
    {
        [SerializeField] private BaseDialogNodeSO m_NextNode;
        [SerializeField] private string m_Receiver;
        [SerializeField] private string m_Action;
        [SerializeField] private ParameterInspectorData[] m_Parameters;

        public BaseDialogNodeSO NextNode => m_NextNode;
        public string Receiver => m_Receiver;
        public string Action => m_Action;
        public ParameterInspectorData[] Parameters => m_Parameters;
        public ParameterInspectorData GetParameter(string paramName)
        {
            return m_Parameters.FirstOrDefault(p => p.ParamName.Equals(paramName));
        }
    }
}