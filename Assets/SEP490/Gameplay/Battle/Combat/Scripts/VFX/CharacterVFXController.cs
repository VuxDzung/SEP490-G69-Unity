namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CharacterVFXController : MonoBehaviour
    {
        [SerializeField] private CharacterVFXConfigSO m_VfxConfig;
        [SerializeField] private List<Transform> m_VfxParentList = new List<Transform>();
        [SerializeField] private ParticleSystem m_AtkVFX;

        private void Awake()
        {
            if (m_VfxConfig == null)
            {
                m_VfxConfig = Resources.Load<CharacterVFXConfigSO>("VFX/CharacterVFXConfig.Effects");
            }
        }

        public void PlayAtkVFX()
        {
            if (m_AtkVFX != null)
            {
                m_AtkVFX.Play();
            }
        }

        public void PlayVfxList(IReadOnlyList<SpawnVfxSettings> vfxList)
        {
            StartCoroutine(SpawnVfxListCoroutine(vfxList));
        }

        private IEnumerator SpawnVfxListCoroutine(IReadOnlyList<SpawnVfxSettings> vfxList)
        {
            foreach (var vfx in vfxList)
            {
                yield return new WaitForSeconds(vfx.data.delay);

                PlayVFXById(vfx.data.vfxId, vfx.onCompleted);
            }
        }

        public void PlayVFXById(string vfxId, Action onCompleted = null)
        {
            if (m_VfxConfig == null)
            {
                Debug.LogError($"[CharacterVFXController.PlayVFXById fatail error] Vfx config in {gameObject.name} is null");
                return;
            }

            VFXData data = m_VfxConfig.GetById(vfxId);

            if (data == null)
            {
                onCompleted?.Invoke();
                return;
            }

            Transform parent = GetParentByName(data.spawnParent);
            if (parent == null)
            {
                parent = this.transform;
            }

            Vector3 position = parent.position + data.spawnOffset;

            Transform vfxTrans = PoolManager.Pools[GameConstants.POOL_COMBAT_VFX].Spawn(data.vfxTransform, position, Quaternion.identity, parent);

            EntityVfxHandler vfxHandler = vfxTrans.GetComponent<EntityVfxHandler>();
            if (vfxHandler != null)
            {
                vfxHandler.SetOnCompletedCallback(() =>
                {
                    onCompleted?.Invoke();
                }).StartPlayVfx();
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