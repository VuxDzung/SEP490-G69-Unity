namespace SEP490G69.Battle.Combat
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(fileName = "intent_")]
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

        public EIntentAction Intent => m_IntentAction;

        public float BaseDamage => m_BaseDamage;
        public float AttackMultiplier => m_AttackMultiplier;

        public float BaseDefend => m_DefendValue;
        public float DefendMultiplier => m_DefendMultiplier; 

        public string GainEffectId => m_GainEffectId;
        public int GainAmount => m_GainAmount;

        public string InflictEffectId => m_InflictEffectId;
        public int InflictAmount => m_InflictAmount;
    }

    [Flags]
    public enum EIntentAction
    {
        None = 0,
        Attack = 1,
        Shield = 2,
        InflictEffect = 3,
        GainEffect = 4,
        AddCardToPlayer = 5,
        Pierce = 6,
    }
}