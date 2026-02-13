namespace SEP490G69
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(
        fileName = "ChoiceNode",
        menuName = OrganizationConstants.NAMESPACE + "/Dialog Framework/Dialog Choice Node")]
    public class ChoiceNodeSO : BaseDialogNodeSO
    {
        [SerializeField] private DialogChoiceData[] m_Choices;

        public DialogChoiceData[] Choices => m_Choices;

        public DialogChoiceData GetChoice(string choiceId)
        {
            return m_Choices.FirstOrDefault(c => c.ChoiceID.Equals(choiceId));
        }
    }

    [System.Serializable]
    public class DialogChoiceData
    {
        [SerializeField] private string m_ChoiceID;      // Localization ID
        [SerializeField] private BaseDialogNodeSO m_NextNode;

        public string ChoiceID => m_ChoiceID;
        public BaseDialogNodeSO NextNode => m_NextNode;
    }
}