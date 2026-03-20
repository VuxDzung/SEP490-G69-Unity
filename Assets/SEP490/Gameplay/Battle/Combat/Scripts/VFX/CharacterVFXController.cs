namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class CharacterVFXController : MonoBehaviour
    {
        [SerializeField] private CharacterVFXConfigSO m_VfxConfig;
        [SerializeField] private ParticleSystem m_AtkVFX;
        [SerializeField] private ParticleSystem m_StunVFX;
        [SerializeField] private ParticleSystem m_BuffEffectVFX;

        private void Awake()
        {
            if (m_VfxConfig == null)
            {
                m_VfxConfig = Resources.Load<CharacterVFXConfigSO>("VFX/CharacterVFXConfig");
            }
        }

        public void PlayAtkVFX()
        {
            if (m_AtkVFX != null)
            {
                m_AtkVFX.Play();
            }
        }

        public void PlayStunVFX()
        {
            if (m_StunVFX != null)
            {
                m_StunVFX.Play();
            }
        }
        public void StopStunVFX()
        {
            if(m_StunVFX != null)
            {
                m_StunVFX.Stop();
            }
        }

        public void PlayBuffVFX()
        {
            if(m_BuffEffectVFX != null) 
            {
                m_BuffEffectVFX.Play();
            }
        }
        public void StopBuffVFX()
        {
            if(m_BuffEffectVFX != null)
            {
                m_BuffEffectVFX.Stop();
            }
        }

        public void PlayVFXById(string vfxId)
        {
            if (m_VfxConfig == null)
            {
                Debug.LogError($"[CharacterVFXController.PlayVFXById fatail error] Vfx config in {gameObject.name} is null");
                return;
            }

            VFXData data = m_VfxConfig.GetById(vfxId);

            if (data == null)
            {
                return;
            }

            Transform vfxTrans = PoolManager.Pools[GameConstants.POOL_COMBAT_VFX].Spawn(data.vfxTransform);
            ParticleSystem vfx = vfxTrans.GetComponent<ParticleSystem>();
            if (vfx != null)
            {
                vfx.Play();
            }
        }

        public void StopVFXId(string vfxId)
        {
            if (m_VfxConfig == null)
            {
                Debug.LogError($"[CharacterVFXController.PlayVFXById fatail error] Vfx config in {gameObject.name} is null");
                return;
            }

            VFXData data = m_VfxConfig.GetById(vfxId);

            if (data == null)
            {
                return;
            }

            if (!PoolManager.Pools[GameConstants.POOL_COMBAT_VFX]
                            .IsDespawned(data.vfxTransform))
            {
                PoolManager.Pools[GameConstants.POOL_COMBAT_VFX].DespawnObject(data.vfxTransform);
            }
        }
    }
}