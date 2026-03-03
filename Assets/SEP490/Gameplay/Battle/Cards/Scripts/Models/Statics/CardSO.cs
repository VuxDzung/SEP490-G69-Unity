namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CardSO_", menuName = OrganizationConstants.NAMESPACE + "/Cards/Card data")]
    public class CardSO : ScriptableObject
    {
        [SerializeField] private string cardId;
        [SerializeField] private string cardName;
        [SerializeField] private string cardDescription;
        [SerializeField] private Sprite icon;
        [SerializeField] private int cost;
        [SerializeField] private float m_BaseDmg;
        [SerializeField] private EStatusType m_ModifyStatType;
        [SerializeField] private EOperator m_DmgModOp;
        [SerializeField] private float dmgModifierValue;
        [SerializeField] private EActionType m_ActionType;
        [SerializeField] private EDamageType m_DamageType;

        [SerializeField] private List<StatusModifierSO> m_RecoverModifiers;

        /// <summary>
        /// Status effects apply for owner
        /// </summary>
        [SerializeField] private List<StatusEffectSO> m_StatusGains;

        /// <summary>
        /// Status effects apply for enemy
        /// </summary>
        [SerializeField] private List<StatusEffectSO> m_StatusInflicts;

        public string CardId => cardId;
        public string CardName => cardName;
        public string CardDescription => cardDescription;
        public Sprite Icon => icon;
        public int Cost => cost;
        public float BaseDmg => m_BaseDmg;
        public EStatusType ModifyStatType => m_ModifyStatType;
        public EOperator DmgModOp => m_DmgModOp;
        public float DmgModifierValue => dmgModifierValue;
        public EActionType ActionType => m_ActionType;
        public EDamageType DamageType => m_DamageType;

        public StatusModifierSO[] RecoverModifiers => m_RecoverModifiers.ToArray();

        public StatusEffectSO[] StatusGains => m_StatusGains.ToArray();
        public StatusEffectSO[] StatusInflicts => m_StatusInflicts.ToArray();
    }
}