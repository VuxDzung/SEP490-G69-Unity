namespace SEP490G69
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections;
    using SEP490G69.Addons.Localization;

    public class TooltipTrigger : MonoBehaviour,
                                  IPointerEnterHandler,
                                  IPointerExitHandler,
                                  IPointerDownHandler,
                                  IPointerUpHandler
    {
        [SerializeField] private string tooltipContentKey;
        [SerializeField] private float holdDuration = 0.7f;

        private Coroutine holdCoroutine;
        private bool isMobilePlatform;

        private TooltipController _tooltipController;
        protected TooltipController TooltipController
        {
            get
            {
                if (_tooltipController == null)
                {
                    _tooltipController = ContextManager.Singleton.ResolveGameContext<TooltipController>();
                }
                return _tooltipController;
            }
        }

        private void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
        isMobilePlatform = true;
#else
            isMobilePlatform = false;
#endif
        }

        // ===============================
        // DESKTOP - HOVER
        // ===============================

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isMobilePlatform) return;

            TooltipController.Show(
                tooltipContentKey,
                eventData.position
            );
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isMobilePlatform) return;

            TooltipController.Hide();
        }

        // ===============================
        // MOBILE - HOLD
        // ===============================

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isMobilePlatform) return;

            holdCoroutine = StartCoroutine(HoldToShow(eventData));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isMobilePlatform) return;

            if (holdCoroutine != null)
                StopCoroutine(holdCoroutine);

            TooltipController.Hide();
        }

        private IEnumerator HoldToShow(PointerEventData eventData)
        {
            yield return new WaitForSeconds(holdDuration);

            TooltipController.Show(
                tooltipContentKey,
                eventData.position
            );
        }
    }
}