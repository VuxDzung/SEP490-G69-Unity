namespace SEP490G69
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CharacterConfig", menuName = OrganizationConstants.NAMESPACE + "/Characters/Character config")]
    public class CharacterConfigSO : ScriptableObject
    {
        [SerializeField] private CharacterDataSO[] m_Characters;

        public CharacterDataSO GetCharacter(string characterID)
        {
            return m_Characters.FirstOrDefault(c => c.CharacterID == characterID);
        }
    }
}