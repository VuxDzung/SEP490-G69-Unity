namespace SEP490G69
{
    using System.Linq;
    using UnityEngine;
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = OrganizationConstants.NAMESPACE + "/Character config")]
    public class CharacterConfigSO : ScriptableObject
    {
        [SerializeField] private BaseCharacterSO[] m_Characters;

        public BaseCharacterSO[] Characters => m_Characters;

        public BaseCharacterSO GetCharacterById(string characterId)
        {
            foreach (BaseCharacterSO character in m_Characters)
            {
                if (character.CharacterId == characterId)
                {
                    return character;
                }
            }
            return null;
        }

        public BaseCharacterSO[] GetCharactersByType(ECharacterType characterType)
        {
            return m_Characters.Where(c => c.CharacterType == characterType).ToArray();  
        }
    }
}