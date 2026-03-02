namespace SEP490G69
{
    using SEP490G69.Addons.Localization;
    using TMPro;
    using UnityEngine;

    public class UITooltipElement : MonoBehaviour, IPooledObject
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private TextMeshProUGUI m_ContentTmp;

        private LocalizationManager _localizeManager;
        protected LocalizationManager LocalizeManager
        {
            get
            {
                if (_localizeManager == null)
                {
                    _localizeManager = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();
                }
                return _localizeManager;
            }
        }

        public void Spawn()
        {

        }
        public void Despawn()
        {
            
        }

        public void SetContent(string contentKey)
        {
            m_ContentTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_TOOL_TIPS, contentKey);
        }
        public void SetPosition(Vector2 screenPosition)
        {
            root.position = screenPosition;
        }
    }
}