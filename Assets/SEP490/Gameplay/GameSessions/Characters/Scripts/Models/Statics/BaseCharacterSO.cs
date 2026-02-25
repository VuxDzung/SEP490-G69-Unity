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
        [SerializeField] private Sprite m_FullBodyImg;

        [SerializeField] private int baseVit;
        [SerializeField] private int basePow;
        [SerializeField] private int baseInt;
        [SerializeField] private int baseSta;
        [SerializeField] private int baseDef;
        [SerializeField] private int baseAgi;
        [SerializeField] private float baseEnergy;
        [SerializeField] private float baseMood;

        [SerializeField] private GameObject m_Prefab;

        public string CharacterId => m_CharacterId;
        public string CharacterName => m_CharacterName;
        public string CharacterDescription => m_CharacterDescription;
        public Sprite Thumbnail => m_Thumbnail;
        public Sprite FullBodyImg => m_FullBodyImg;

        public int BaseVit => baseVit;
        public int BasePow => basePow;
        public int BaseInt => baseInt;
        public int BaseSta => baseSta;
        public int BaseDef => baseDef;
        public int BaseAgi => baseAgi;
        public float BaseEnergy => baseEnergy;
        public float BaseMood => baseMood;
        public GameObject Prefab => m_Prefab;
    }
}