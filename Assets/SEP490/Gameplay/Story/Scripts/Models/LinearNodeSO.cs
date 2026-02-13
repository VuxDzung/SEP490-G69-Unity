namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "LinearNode", menuName = OrganizationConstants.NAMESPACE + "/Dialog Framework/Dialog Linear Node")]
    public class LinearNodeSO : BaseDialogNodeSO
    {
        [SerializeField] private BaseDialogNodeSO m_NextNode;

        public BaseDialogNodeSO NextNode => m_NextNode;
    }
}