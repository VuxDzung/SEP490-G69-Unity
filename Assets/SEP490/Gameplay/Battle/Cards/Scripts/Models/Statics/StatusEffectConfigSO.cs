namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "StatusEffectConfig", menuName = OrganizationConstants.NAMESPACE + "/Battle/Status Effects/Status Effects Config")]
    public class StatusEffectConfigSO : ScriptableObject
    {
        [SerializeField] private List<StatusEffectSO> m_Effects;

        public StatusEffectSO GetById(string effectId)
        {
            if (string.IsNullOrEmpty(effectId) || m_Effects.Count == 0)
            {
                return null;
            }
            return m_Effects.FirstOrDefault(x => x.EffectId == effectId);
        }
    }
}