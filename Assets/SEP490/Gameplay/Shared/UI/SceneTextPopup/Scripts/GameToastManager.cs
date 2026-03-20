namespace SEP490G69
{
    using UnityEngine;

    public class GameToastManager : GlobalSingleton<GameToastManager>
    {
        [SerializeField] private Canvas m_ToastCanvas;
        [SerializeField] private GameObject m_Prefab;

        protected override void CreateNewInstance()
        {
            base.CreateNewInstance();
            SetupCanvas();
        }

        public void SpawnToast(string message, Vector3 position, Color color, float aliveTime = 3f)
        {
            SetupCanvas();

            Transform toastTrans = PoolManager.Pools[GameConstants.POOL_TOAST].Spawn(m_Prefab, position, Quaternion.identity, m_ToastCanvas.transform);
            SceneUITextToast toast = toastTrans.GetComponent<SceneUITextToast>();
            if (toast != null)
            {
                toast.SetMessage(message, color, aliveTime);
            }
        }

        private void SetupCanvas()
        {
            if (m_ToastCanvas != null)
            {
                if (m_ToastCanvas.renderMode == RenderMode.ScreenSpaceCamera ||
                    m_ToastCanvas.worldCamera == null)
                {
                    m_ToastCanvas.worldCamera = Camera.main;
                }
            }
            else
            {
                Debug.LogError($"[GameToastManager.SetupCanvas fatail error] Canvas is null");
            }
        }
    }
}