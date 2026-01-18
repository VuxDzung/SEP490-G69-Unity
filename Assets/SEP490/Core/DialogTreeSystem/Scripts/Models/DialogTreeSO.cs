namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "DialogTree", menuName = OrganizationConstants.NAMESPACE + "/Dialog Framework/Dialog Tree")]
    public class DialogTreeSO : ScriptableObject
    {
        [SerializeField] private string m_TreeId;
        [SerializeField] private string m_StarterNodeID;
        [SerializeField] private List<BaseDialogNodeSO> m_NodeList = new List<BaseDialogNodeSO>();

        public string TreeID => m_TreeId;
        public string StarterNodeID => m_StarterNodeID;
        public BaseDialogNodeSO[] Nodes => m_NodeList.ToArray();

        public BaseDialogNodeSO GetStarterNode()
        {
            if (string.IsNullOrEmpty(m_StarterNodeID)) return null;

            return GetNode(m_StarterNodeID);
        }

        public BaseDialogNodeSO GetNode(string nodeId)
        {
            return Nodes.FirstOrDefault(n => n.NodeID.Equals(nodeId));
        }
    }
}