namespace SEP490G69
{
    using System.Collections;
    using UnityEngine;

    public class SpawnToastSettingsData
    {
        public string Message { get; set; } = string.Empty;
        public Color TextColor { get; set; } = Color.white;
        public float TextSize { get; set; } = 15f;

        public Vector3 SpawnPosition { get; set; }
        public float DelaySpawnTime { get; set; } = 0.1f;
        public float EndYDistance { get; set; } = 1f;
        public float AliveTime { get; set; } = 1f;
    }

    public class GameToastManager : GlobalSingleton<GameToastManager>
    {
        [SerializeField] private Canvas m_ToastCanvas;
        [SerializeField] private GameObject m_Prefab;

        protected override void CreateNewInstance()
        {
            base.CreateNewInstance();
            SetupCanvas();
        }

        public void SpawnToast(SpawnToastSettingsData spawnData)
        {
            SetupCanvas();

            StartCoroutine(DelaySpawnToast(spawnData));
        }

        private IEnumerator DelaySpawnToast(SpawnToastSettingsData spawnData)
        {
            yield return new WaitForSeconds(spawnData.DelaySpawnTime);

            Transform toastTrans = PoolManager.Pools[GameConstants.POOL_TOAST].Spawn(m_Prefab, spawnData.SpawnPosition, Quaternion.identity, m_ToastCanvas.transform);
            SceneUITextToast toast = toastTrans.GetComponent<SceneUITextToast>();
            if (toast != null)
            {
                toast.SetMessage(spawnData.Message, spawnData.TextColor, spawnData.AliveTime, spawnData.EndYDistance, spawnData.TextSize);
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