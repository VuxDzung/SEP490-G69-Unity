namespace SEP490G69
{
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CharacterSO_", menuName = OrganizationConstants.NAMESPACE + "/Characters/Base character data")]
    public class BaseCharacterSO : ScriptableObject
    {
        [Header("General information")]
        [SerializeField] private string m_CharacterId;
        [SerializeField] private bool m_IsAvailable = true;
        [SerializeField] private string m_CharacterName;
        [SerializeField] private string m_CharacterDescription;
        [SerializeField] private Sprite m_Thumbnail;
        [SerializeField] private Sprite m_FullBodyImg;
        [SerializeField] private GameObject m_Prefab;
        [SerializeField] private GameObject m_CombatPrefab;
        [SerializeField] private GameObject m_RunningPrefab;
        [SerializeField] private ECharacterType m_CharacterType;
        [SerializeField] private EAttackType m_AtkType;

        [Header("Character stats")]
        [SerializeField] private int baseVit;
        [SerializeField] private int basePow;
        [SerializeField] private int baseInt;
        [SerializeField] private int baseSta;
        [SerializeField] private int baseDef;
        [SerializeField] private int baseAgi;
        [SerializeField] private float baseEnergy;
        [SerializeField] private float baseMood;
        [SerializeField] private int baseRP = 0;

        [Header("Sfx")]
        [SerializeField] private string m_MeleeSfxId;
        [SerializeField] private string m_RangedSfxId;

        public string CharacterId => m_CharacterId;
        public bool IsAvailable => m_IsAvailable;
        public string CharacterName => m_CharacterName;
        public string CharacterDescription => m_CharacterDescription;
        public Sprite Thumbnail => m_Thumbnail;
        public Sprite FullBodyImg => m_FullBodyImg;
        public ECharacterType CharacterType => m_CharacterType;
        public EAttackType AtkType => m_AtkType;
        public int BaseVit => baseVit;
        public int BasePow => basePow;
        public int BaseInt => baseInt;
        public int BaseSta => baseSta;
        public int BaseDef => baseDef;
        public int BaseAgi => baseAgi;
        public float BaseEnergy => baseEnergy;
        public float BaseMood => baseMood;
        public int BaseRP => baseRP;

        public string MeleeSfxId => m_MeleeSfxId;
        public string RangedSfxId => m_RangedSfxId;

        public GameObject CombatPrefab => m_CombatPrefab;
        public GameObject Prefab => m_Prefab;
        public GameObject RunningPrefab => m_RunningPrefab;

        public T ConvertAs<T>() where T : BaseCharacterSO
        {
            return this as T;
        }
    }
}