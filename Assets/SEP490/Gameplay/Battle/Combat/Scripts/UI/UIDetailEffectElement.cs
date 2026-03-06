namespace SEP490G69.Battle.Combat
{
    using TMPro;
    using UnityEngine;

    public class UIDetailEffectElement : UIStatusEffectElement
    {
        [SerializeField] private TextMeshProUGUI m_EffectNameTmp;
        [SerializeField] private TextMeshProUGUI m_EffectDescsTmp;


        public UIDetailEffectElement SetEffectName(string effectName)
        {
            m_EffectNameTmp.text = effectName;
            return this;
        }
        public UIDetailEffectElement SetEffectDescs(string effectDescs)
        {
            m_EffectDescsTmp.text = effectDescs;
            return this;
        }

        public override void Spawn()
        {
            base.Spawn();
        }
        public override void Despawn()
        {
            base.Despawn();
            m_EffectNameTmp.text = string.Empty;
            m_EffectDescsTmp.text= string.Empty;
        }
    }
}