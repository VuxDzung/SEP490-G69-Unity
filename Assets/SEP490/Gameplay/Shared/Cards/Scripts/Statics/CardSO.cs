namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "CardSO_", menuName = OrganizationConstants.NAMESPACE + "/Cards/Card data")]
    public class CardSO : ScriptableObject
    {
        [SerializeField] private string cardId;
        [SerializeField] private string cardName;
        [SerializeField] private string cardDescription;
        [SerializeField] private Sprite icon;
        [SerializeField] private int cost;

        public string CardId => cardId;
        public string CardName => cardName;
        public string CardDescription => cardDescription;
        public Sprite Icon => icon;
        public int Cost => cost;
    }
}