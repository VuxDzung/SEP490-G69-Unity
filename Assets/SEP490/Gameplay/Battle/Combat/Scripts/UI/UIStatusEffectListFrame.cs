namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIStatusEffectListFrame : GameUIFrame
    {
        [SerializeField] private Button m_CloseBtn;
        [SerializeField] private Transform m_EffectDetailsPrefab;
        [SerializeField] private Transform m_EffectContainer;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_CloseBtn.onClick.AddListener(Close);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_CloseBtn.onClick.RemoveListener(Close);
        }

        private void Close()
        {
            UIManager.HideFrame(FrameId);
            ClearAll();
        }

        public void LoadStatusEffects(IReadOnlyList<RuntimeStatusEffect> effects)
        {
            ClearAll();

            // Spawn here.
            foreach (var effect in effects)
            {
                Transform effectUITrans = PoolManager.Pools[GameConstants.POOL_UI_STATUS_EFFECT_DETAILS]
                                                     .Spawn(m_EffectDetailsPrefab, m_EffectContainer);

                UIDetailEffectElement effectElement = effectUITrans.GetComponent<UIDetailEffectElement>();
                if (effectElement != null)
                {
                    effectElement.SetEffectName(effect.Data.EffectName)
                                 .SetEffectDescs(effect.Data.EffectDesc)
                                 .SetId(effect.Data.EffectId)
                                 .SetImg(effect.Data.Icon)
                                 .SetRemainAmount(effect.Stack);
                }
            }
        }

        private void ClearAll()
        {
            if (!PoolManager.Pools[GameConstants.POOL_UI_STATUS_EFFECT_DETAILS].IsEmpty)
            {
                PoolManager.Pools[GameConstants.POOL_UI_STATUS_EFFECT_DETAILS].DespawnAll();
            }
        }
    }
}