namespace SEP490G69
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "CharacterData_", menuName = OrganizationConstants.NAMESPACE + "/Characters/Character data")]
    public class CharacterDataSO : ScriptableObject
    {
        [SerializeField] private string m_CharacterID;
        [SerializeField] private string m_CharacterName;
        [SerializeField] private string m_CharacterDescription;
        [SerializeField] private Sprite m_Avatar;

        [SerializeField] private StatusDataSO[] m_StatusArray;

        public string CharacterID => m_CharacterID;
        public string CharacterName => m_CharacterName;
        public string CharacterDescription => m_CharacterDescription;
        public Sprite Avatar => m_Avatar;
    }
}