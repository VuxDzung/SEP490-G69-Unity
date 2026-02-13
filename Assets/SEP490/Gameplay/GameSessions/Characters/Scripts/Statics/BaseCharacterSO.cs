namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "CharacterSO_", menuName = OrganizationConstants.NAMESPACE + "/Character data")]
    public class BaseCharacterSO : ScriptableObject
    {
        [SerializeField] private string m_CharacterId;
        [SerializeField] private string m_CharacterName;
        [SerializeField] private string m_CharacterDescription;
        [SerializeField] private Sprite m_Thumbnail;

        public string CharacterId => m_CharacterId;
        public string CharacterName => m_CharacterName;
        public string CharacterDescription => m_CharacterDescription;
        public Sprite Thumbnail => m_Thumbnail;
    }
}