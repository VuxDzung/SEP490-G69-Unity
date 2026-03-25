namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CharacterVFXController : MonoBehaviour
    {
        [SerializeField] private CharacterVFXConfigSO m_VfxConfig;
        [SerializeField] private List<Transform> m_VfxParentList = new List<Transform>();
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

        public void PlayVfxList(IReadOnlyList<CardSpawnVfxData> vfxList)
        {
            StartCoroutine(SpawnVfxListCoroutine(vfxList));
        }

        private IEnumerator SpawnVfxListCoroutine(IReadOnlyList<CardSpawnVfxData> vfxList)
        {
            foreach (var vfx in vfxList)
            {
                yield return new WaitForSeconds(vfx.delay);

                PlayVFXById(vfx.vfxId);
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

            Transform parent = GetParentByName(data.spawnParent);
            if (parent == null)
            {
                parent = this.transform;
            }

            Vector3 position = parent.position + data.spawnOffset;

            Transform vfxTrans = PoolManager.Pools[GameConstants.POOL_COMBAT_VFX].Spawn(data.vfxTransform, position, Quaternion.identity, parent);
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
                            .IsDespawned(data.vfxTransform.gameObject))
            {
                PoolManager.Pools[GameConstants.POOL_COMBAT_VFX].DespawnObject(data.vfxTransform);
            }
        }

        private Transform GetParentByName(string vfxParentName)
        {
            if (string.IsNullOrEmpty(vfxParentName))
            {
                return null;
            }
            return m_VfxParentList.FirstOrDefault(parent => parent.gameObject.name == vfxParentName);
        }
    }
}