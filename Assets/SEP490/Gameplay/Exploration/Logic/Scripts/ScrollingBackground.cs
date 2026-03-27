namespace SEP490G69
{
    using System;
    using UnityEngine;

    public class ScrollingBackground : MonoBehaviour
    {
        [Header("Scroll Settings")]
        [SerializeField] private Vector2 scrollSpeed = new Vector2(0.1f, 0f);
        [SerializeField] private Renderer m_Renderer;

        private Vector2 _offset;

        private Action _onUpdate;

        private void Awake()
        {
        }

        public void OpenBg()
        {
            m_Renderer.gameObject.SetActive(true);
        }
        public void CloseBg()
        {
            m_Renderer.gameObject.SetActive(false);
        }

        public void SetMaterial(Material mat)
        {
            m_Renderer.material = mat;
        }

        public void StartScrolling()
        {
            _onUpdate = OnScrolling;
        }
        public void StopScrolling()
        {
            _onUpdate = null;
        }

        private void Update()
        {
            if (_onUpdate != null) _onUpdate();
        }

        private void OnScrolling()
        {
            _offset += scrollSpeed * Time.deltaTime;
            m_Renderer.material.mainTextureOffset = _offset;
        }
    }
}