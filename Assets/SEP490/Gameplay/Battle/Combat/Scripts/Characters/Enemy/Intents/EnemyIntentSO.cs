namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "intent_", menuName = OrganizationConstants.NAMESPACE + "/Battle/Enemy/Enemy Intent Action")]
    public class EnemyIntentSO : ScriptableObject
    {
        [SerializeField] private EIntentAction m_IntentAction;

        [Header("Attack intent")]
        [SerializeField] private float m_BaseDamage;
        [SerializeField] private float m_AttackMultiplier;

        [Header("Defend intent")]
        [SerializeField] private float m_DefendValue;
        [SerializeField] private float m_DefendMultiplier;

        [Header("Gain effect intent")]
        [SerializeField] private string m_GainEffectId;
        [SerializeField] private int m_GainAmount;

        [Header("Inflict effect intent")]
        [SerializeField] private string m_InflictEffectId;
        [SerializeField] private int m_InflictAmount;

        [Header("Vfx")]
        [SerializeField] private List<SpawnVfxData> m_VfxList;

        public EIntentAction Intent => m_IntentAction;

        public float BaseDamage => m_BaseDamage;
        public float AttackMultiplier => m_AttackMultiplier;

        public float BaseDefend => m_DefendValue;
        public float DefendMultiplier => m_DefendMultiplier; 

        public string GainEffectId => m_GainEffectId;
        public int GainAmount => m_GainAmount;

        public string InflictEffectId => m_InflictEffectId;
        public int InflictAmount => m_InflictAmount;

        public IReadOnlyList<SpawnVfxData> VfxList => m_VfxList;
    }

    [Flags]
    public enum EIntentAction
    {
        None = 0,
        Attack = 1 << 0,          // 1
        Shield = 1 << 1,          // 2
        InflictEffect = 1 << 2,   // 4
        GainEffect = 1 << 3,      // 8
        AddCardToPlayer = 1 << 4, // 16
        Pierce = 1 << 5,          // 32
    }
}