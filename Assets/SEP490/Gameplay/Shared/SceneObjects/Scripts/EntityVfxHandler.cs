namespace SEP490G69
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class EntityVfxHandler : MonoBehaviour, IPooledObject
    {
        [SerializeField] private float m_AliveTime;

        private Action _onCompleted;

        private ParticleSystem _particleSystem;

        public void Spawn()
        {
            if (_particleSystem == null) _particleSystem = GetComponentInChildren<ParticleSystem>();
            StartPlayVfx();
        }
        public void Despawn()
        {
            _particleSystem.Stop();
            _onCompleted = null;
        }

        public EntityVfxHandler SetOnCompletedCallback(Action onCompleted)
        {
            _onCompleted = onCompleted;
            return this;
        }

        public void StartPlayVfx()
        {
            _particleSystem.Play();
            StartCoroutine(DelayDespawn());
        }

        private IEnumerator DelayDespawn()
        {
            yield return new WaitForSeconds(m_AliveTime);

            _onCompleted?.Invoke();

            PoolManager.Pools[GameConstants.POOL_COMBAT_VFX].DespawnObject(this.transform);
        }
    }
}