namespace SEP490G69
{
    using UnityEngine;

    public class TooltipController : MonoBehaviour, IGameContext
    {
        [SerializeField] private UITooltipElement tooltipView;
        [SerializeField] private Vector2 offset = new Vector2(15f, -15f);

        public void Show(string content, Vector2 position)
        {
            tooltipView.SetContent(content);
            tooltipView.SetPosition(position + offset);
            if (!tooltipView.gameObject.activeSelf) tooltipView.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (tooltipView.gameObject.activeSelf)
                tooltipView.gameObject.SetActive(false);
        }

        public void SetManager(ContextManager manager)
        {
            
        }
    }
}