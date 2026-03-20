namespace SEP490G69
{
    using TMPro;
    using UnityEngine;
    using System.Collections;
    using DG.Tweening;

    public class SceneUITextToast : MonoBehaviour, IPooledObject
    {
        [SerializeField] private TextMeshProUGUI m_MsgTmp;

        private float _aliveTime;

        public void Spawn()
        {
            if (m_MsgTmp == null)
            {
                m_MsgTmp = GetComponent<TextMeshProUGUI>();
            }
        }
        public void Despawn()
        {
            if(m_MsgTmp != null)
            {
                m_MsgTmp.text = string.Empty;
            }
        }

        public void SetMessage(string message, Color color, float aliveTime)
        {
            if (m_MsgTmp != null)
            {
                m_MsgTmp.color = color;
                m_MsgTmp.text = message;
                _aliveTime = aliveTime;

                float targetY = transform.position.y + 3f;

                transform.DOMoveY(targetY, _aliveTime).OnComplete(() =>
                {
                    PoolManager.Pools[GameConstants.POOL_TOAST].DespawnObject(this.transform);
                });
            }
        }
    }
}