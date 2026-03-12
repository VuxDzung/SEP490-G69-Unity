namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICardDetailFrame : GameUIFrame
    {
        [Header("Overlay (Click outside to close)")]
        [SerializeField] private Button m_BackgroundOverlayBtn;

        [Header("Card Visual Information")]
        [SerializeField] private Image m_CardIcon;
        [SerializeField] private TextMeshProUGUI m_CardNameTmp;
        [SerializeField] private TextMeshProUGUI m_CardDescTmp;
        [SerializeField] private TextMeshProUGUI m_CardCostTmp;

        [Header("Tooltips")]
        [SerializeField] private Transform m_DescriptionContainer;
        [SerializeField] private GameObject m_StatusTooltipPrefab;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            // Gắn sự kiện đóng Popup khi click vào vùng mờ
            if (m_BackgroundOverlayBtn != null)
            {
                m_BackgroundOverlayBtn.onClick.AddListener(HideThisView);
            }
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            // Gỡ sự kiện khi đóng để tránh tràn bộ nhớ
            if (m_BackgroundOverlayBtn != null)
            {
                m_BackgroundOverlayBtn.onClick.RemoveListener(HideThisView);
            }
        }

        /// <summary>
        /// Hàm này được gọi từ các màn hình khác (như màn Chọn nhân vật) để truyền Data vào
        /// </summary>
        public void LoadData(CardSO cardData)
        {
            if (cardData == null)
            {
                HideThisView();
                return;
            }

            // 1. Gán thông tin trực quan của thẻ bài
            if (m_CardIcon != null) m_CardIcon.sprite = cardData.Icon;
            if (m_CardNameTmp != null) m_CardNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, cardData.CardName);
            if (m_CardDescTmp != null) m_CardDescTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, cardData.CardDescription);
            if (m_CardCostTmp != null) m_CardCostTmp.text = cardData.Cost.ToString();

            // 2. Tự động quét và Load các Tooltip giải thích trạng thái bằng PoolManager
            LoadStatusTooltips(cardData);
        }

        private void LoadStatusTooltips(CardSO cardData)
        {
            Debug.Log($"<color=yellow>[Tooltip Debug]</color> 1. Bắt đầu load Tooltip cho thẻ: {cardData.CardName}");

            // 1. Dọn dẹp Tooltip UI cũ
            string poolKey = "UIStatusTooltip";
            if (PoolManager.Pools.ContainsKey(poolKey))
            {
                PoolManager.Pools[poolKey].DespawnAll();
            }
            else
            {
                // Fallback nếu Pool chưa cấu hình
                for (int i = m_DescriptionContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(m_DescriptionContainer.GetChild(i).gameObject);
                }
            }

            if (m_StatusTooltipPrefab == null)
            {
                Debug.LogError("<color=red>[Tooltip Debug]</color> LỖI: Chưa kéo m_StatusTooltipPrefab vào Inspector!");
                return;
            }
            if (m_DescriptionContainer == null)
            {
                Debug.LogError("<color=red>[Tooltip Debug]</color> LỖI: Chưa kéo m_DescriptionContainer vào Inspector!");
                return;
            }

            // Gộp tất cả Status Effect mà thẻ bài này có thể Gây ra (Inflicts) hoặc Nhận được (Gains)
            var allStatuses = new List<StatusEffectSO>();

            // Try-catch đề phòng trường hợp CardSO chưa khởi tạo List gây lỗi ToArray()
            try
            {
                if (cardData.StatusGains != null)
                {
                    allStatuses.AddRange(cardData.StatusGains);
                    Debug.Log($"<color=yellow>[Tooltip Debug]</color> 2. Thẻ có {cardData.StatusGains.Length} hiệu ứng Gains.");
                }

                if (cardData.StatusInflicts != null)
                {
                    allStatuses.AddRange(cardData.StatusInflicts);
                    Debug.Log($"<color=yellow>[Tooltip Debug]</color> 3. Thẻ có {cardData.StatusInflicts.Length} hiệu ứng Inflicts.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"<color=red>[Tooltip Debug]</color> LỖI khi đọc mảng Status từ CardSO: {e.Message}");
            }

            // Lọc trùng và LOẠI BỎ các phần tử NULL (nếu Inspector bị để trống)
            var uniqueStatuses = allStatuses.Where(s => s != null).Distinct().ToList();

            Debug.Log($"<color=yellow>[Tooltip Debug]</color> 4. Tổng số Tooltip trạng thái cần hiển thị (sau khi lọc trùng & null): {uniqueStatuses.Count}");

            if (uniqueStatuses.Count == 0)
            {
                Debug.Log($"<color=yellow>[Tooltip Debug]</color> 5. Thẻ này KHÔNG CÓ Status Effect nào. Kết thúc load.");
                return; // Dừng luôn nếu không có gì để hiện
            }

            int spawnedCount = 0;

            // Sinh Prefab Tooltip cho từng Status
            foreach (var status in uniqueStatuses)
            {
                Debug.Log($"<color=yellow>[Tooltip Debug]</color> ---> Đang tiến hành spawn Tooltip cho: {status.EffectName}");
                Transform tooltipTrans = null;

                // Cố gắng Spawn bằng PoolManager trước
                if (PoolManager.Pools.ContainsKey(poolKey))
                {
                    tooltipTrans = PoolManager.Pools[poolKey].Spawn(m_StatusTooltipPrefab.transform, m_DescriptionContainer);
                }
                else
                {
                    // Fallback
                    Debug.LogWarning($"<color=orange>[Tooltip Debug]</color> PoolManager không chứa key '{poolKey}', dùng Instantiate mặc định...");
                    tooltipTrans = Instantiate(m_StatusTooltipPrefab, m_DescriptionContainer).transform;
                }

                if (tooltipTrans == null)
                {
                    Debug.LogError($"<color=red>[Tooltip Debug]</color> LỖI: Spawn thất bại cho trạng thái {status.EffectName}");
                    continue;
                }

                UIStatusTooltipElement tooltipElement = tooltipTrans.GetComponent<UIStatusTooltipElement>();
                if (tooltipElement != null)
                {
                    string locName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_STATUS_EFFECT_NAMES, status.EffectName);
                    string locDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_STATUS_EFFECT_DESC, status.EffectDesc);
                    spawnedCount++;
                    tooltipElement.Setup(status.Icon, locName, locDesc);
                    Debug.Log($"<color=green>[Tooltip Debug]</color> ---> Đổ data THÀNH CÔNG cho Tooltip: {status.EffectName}");

                    
                }
                else
                {
                    Debug.LogError($"<color=red>[Tooltip Debug]</color> LỖI: Prefab Tooltip KHÔNG GẮN script 'UIStatusTooltipElement'!");
                }
            }

            Debug.Log($"<color=yellow>[Tooltip Debug]</color> 6. Hoàn tất quá trình Load Tooltip. Tổng cộng đã spawn: {spawnedCount}");
        }
    }
}