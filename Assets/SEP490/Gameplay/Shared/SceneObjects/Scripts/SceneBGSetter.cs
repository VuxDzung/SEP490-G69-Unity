namespace SEP490G69
{
    using UnityEngine;

    public class SceneBGSetter : MonoBehaviour, ISceneContext
    {
        [SerializeField] private string m_DefaultBgId;
        [SerializeField] private SpriteRenderer m_BgRenderer;

        private BackgroundConfigSO _bgConfig;
        protected BackgroundConfigSO BgConfig
        {
            get
            {
                if (_bgConfig == null)
                {
                    _bgConfig = ContextManager.Singleton.GetDataSO<BackgroundConfigSO>();
                }
                return _bgConfig;
            }
        }

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        public void SetBgById(string bgId)
        {
            if (BgConfig != null)
            {
                BackgroundData data = BgConfig.GetById(bgId);
                if (data != null)
                {
                    m_BgRenderer.sprite = data.BgSprite;
                }
            }
        }
    }
}