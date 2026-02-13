namespace SEP490G69
{
    using System.Linq;
    using UnityEngine;
    [CreateAssetMenu(fileName = "DialogTreeConfig", menuName = OrganizationConstants.NAMESPACE + "/Dialog Framework/Dialog Tree config")]
    public class DialogTreeConfigSO : ScriptableObject
    {
        [SerializeField] private DialogTreeSO[] m_Trees;

        public DialogTreeSO GetTree(string treeId)
        {
            return m_Trees.FirstOrDefault(t => t.TreeID.Equals(treeId));
        }
    }
}