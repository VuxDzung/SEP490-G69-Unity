namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CardConfig", menuName = OrganizationConstants.NAMESPACE + "/Cards/Card config")]
    public class CardConfigSO : ScriptableObject
    {
        [SerializeField] private List<CardSO> m_Cards;

        public CardSO[] Cards => m_Cards.ToArray();

        public CardSO GetCardById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return m_Cards.FirstOrDefault(c => c.CardId == id);
        }
    }
}