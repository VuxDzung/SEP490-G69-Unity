namespace SEP490G69.GameSessions
{
    using System.Collections.Generic;
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Addons.Localization;
    using SEP490G69.Battle.Cards;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICharacterSelectionFrame : GameUIFrame
    {
        [Header("Prefabs & Containers")]
        [SerializeField] private Transform m_UICharacterPrefab;
        [SerializeField] private Transform m_CharacterContainer;

        [Header("Character Info (Top Level)")]
        [SerializeField] private TextMeshProUGUI m_CharacterNameTmp;

        [Header("Tab System")]
        [SerializeField] private Button m_BtnTabStats;
        [SerializeField] private Button m_BtnTabDescription;
        [SerializeField] private GameObject m_BaseStatsContainer;
        [SerializeField] private GameObject m_UniqueSkillContainer;

        [Header("Description Tab")]
        [SerializeField] private TextMeshProUGUI m_CharacterDetailsTmp;

        [Header("Stats Tab - Base Value Fields")]
        [SerializeField] private TextMeshProUGUI m_TmpVitValue;
        [SerializeField] private TextMeshProUGUI m_TmpPowValue;
        [SerializeField] private TextMeshProUGUI m_TmpAgiValue;
        [SerializeField] private TextMeshProUGUI m_TmpIntValue;
        [SerializeField] private TextMeshProUGUI m_TmpStaValue;
        [SerializeField] private TextMeshProUGUI m_TmpDefValue;

        [Header("Stats Tab - Growth Rate Fields")] // THÊM MỚI: Biến cho Growth Rate
        [SerializeField] private TextMeshProUGUI m_TmpVitGrowthRate;
        [SerializeField] private TextMeshProUGUI m_TmpPowGrowthRate;
        [SerializeField] private TextMeshProUGUI m_TmpAgiGrowthRate;
        [SerializeField] private TextMeshProUGUI m_TmpIntGrowthRate;
        [SerializeField] private TextMeshProUGUI m_TmpStaGrowthRate;
        [SerializeField] private TextMeshProUGUI m_TmpDefGrowthRate;

        [Header("Stats Tab - Unique Cards")]
        [SerializeField] private Transform m_CardContainer;
        [SerializeField] private GameObject m_UICardPrefab;

        [Header("Navigation Buttons")]
        [SerializeField] private Button m_PrevCharBtn;
        [SerializeField] private Button m_NextCharBtn;
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private Button m_NextBtn;

        private CharacterConfigSO _characterConfig;
        private GameSessionController _sessionController;

        private CharacterConfigSO CharacterConfig
        {
            get
            {
                if (_characterConfig == null)
                {
                    _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
                }
                return _characterConfig;
            }
        }
        private GameSessionController SessionController
        {
            get
            {
                if (_sessionController == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext<GameSessionController>(out _sessionController);
                }
                return _sessionController;
            }
        }

        private PlayerDataDAO _playerDAO;
        private PlayerDataDAO PlayerDAO
        {
            get
            {
                if (_playerDAO == null)
                {
                    _playerDAO = new PlayerDataDAO();
                }
                return _playerDAO;
            }
        }

        private GameAuthManager _authManager;
        private GameAuthManager AuthManager
        {
            get
            {
                if (_authManager == null)
                {
                    _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
                }
                return _authManager;
            }
        }

        private int _currentCharIndex;
        private string _currentCharId;
        private List<BaseCharacterSO> _characterList = new List<BaseCharacterSO>();

        protected override void OnFrameShown()
        {
            base.OnFrameShown();

            // Gắn sự kiện nút cơ bản
            //m_PrevCharBtn.onClick.AddListener(ShowPrevChar);
            //m_NextCharBtn.onClick.AddListener(ShowNextChar);
            m_BackBtn.onClick.AddListener(Back);
            m_NextBtn.onClick.AddListener(Next);

            // Gắn sự kiện Tab
            m_BtnTabStats.onClick.AddListener(ShowStatsTab);
            m_BtnTabDescription.onClick.AddListener(ShowDescriptionTab);

            // Mặc định hiển thị tab Stats
            ShowStatsTab();

            LoadCharacters();
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();

            //m_PrevCharBtn.onClick.RemoveListener(ShowPrevChar);
            //m_NextCharBtn.onClick.RemoveListener(ShowNextChar);
            m_BackBtn.onClick.RemoveListener(Back);
            m_NextBtn.onClick.RemoveListener(Next);

            m_BtnTabStats.onClick.RemoveListener(ShowStatsTab);
            m_BtnTabDescription.onClick.RemoveListener(ShowDescriptionTab);

            ClearAllChars();
        }

        // ================= TAB LOGIC =================
        private void ShowStatsTab()
        {
            if (m_BaseStatsContainer != null) m_BaseStatsContainer.SetActive(true);
            if (m_UniqueSkillContainer != null) m_UniqueSkillContainer.SetActive(true);

            if (m_CharacterDetailsTmp != null) m_CharacterDetailsTmp.gameObject.SetActive(false);
        }

        private void ShowDescriptionTab()
        {
            if (m_BaseStatsContainer != null) m_BaseStatsContainer.SetActive(false);
            if (m_UniqueSkillContainer != null) m_UniqueSkillContainer.SetActive(false);

            if (m_CharacterDetailsTmp != null) m_CharacterDetailsTmp.gameObject.SetActive(true);
        }
        // =============================================

        private void LoadCharacters()
        {
            _characterList.Clear();
            _characterList.AddRange(CharacterConfig.GetCharactersByType(ECharacterType.Playable));

            foreach (BaseCharacterSO characterData in _characterList)
            {
                Transform charUITrans = PoolManager.Pools["UICharacter"].Spawn(m_UICharacterPrefab, m_CharacterContainer);
                UICharacterElement characterUI = charUITrans.GetComponent<UICharacterElement>();
                if (characterUI != null)
                {
                    characterUI.SetSelectCallback(SelectCharacter);
                    characterUI.SetContent(characterData.CharacterId, characterData.Thumbnail);
                }
            }

            if (_characterList.Count > 0)
            {
                _currentCharIndex = 0;
                BaseCharacterSO firstChar = _characterList[0];
                SelectCharacter(firstChar.CharacterId);
            }
        }

        private void ClearAllChars()
        {
            if (PoolManager.Pools.ContainsKey("UICharacter") && PoolManager.Pools["UICharacter"].Count > 0)
            {
                PoolManager.Pools["UICharacter"].DespawnAll();
            }
        }

        private void SelectCharacter(string characterId)
        {
            if (string.IsNullOrEmpty(characterId)) return;

            _currentCharId = characterId;
            BaseCharacterSO characterSO = CharacterConfig.GetCharacterById(characterId);

            if (characterSO == null)
            {
                Debug.LogError("Character is not configured or is not available right now");
                return;
            }

            // 1. Tên & Mô tả
            if (m_CharacterNameTmp != null) m_CharacterNameTmp.text = characterSO.CharacterName;
            if (m_CharacterDetailsTmp != null) m_CharacterDetailsTmp.text = LocalizeManager.GetText("CharacterDescs", characterSO.CharacterDescription);

            // 2. Base Stats
            if (m_TmpVitValue != null) m_TmpVitValue.text = characterSO.BaseVit.ToString();
            if (m_TmpPowValue != null) m_TmpPowValue.text = characterSO.BasePow.ToString();
            if (m_TmpAgiValue != null) m_TmpAgiValue.text = characterSO.BaseAgi.ToString();
            if (m_TmpIntValue != null) m_TmpIntValue.text = characterSO.BaseInt.ToString();
            if (m_TmpStaValue != null) m_TmpStaValue.text = characterSO.BaseSta.ToString();
            if (m_TmpDefValue != null) m_TmpDefValue.text = characterSO.BaseDef.ToString();

            // 3. Unique Cards & Growth Rate (Cần PlayerCharacterDataSO)
            LoadUniqueCardsAndGrowthRate(characterSO);

            // 4. Spawn Model
            CharacterSpawnHandler.Instance.SpawnCharacter(characterSO.Prefab);
        }

        private void LoadUniqueCardsAndGrowthRate(BaseCharacterSO baseChar)
        {
            Debug.Log($"<color=cyan>[Card Load Debug]</color> 1. Bắt đầu load cho nhân vật: {baseChar.CharacterId}");

            // 1. Dọn dẹp Card UI cũ bằng PoolManager thay vì Destroy
            string poolKey = "UIUniqueCard";
            if (PoolManager.Pools.ContainsKey(poolKey))
            {
                PoolManager.Pools[poolKey].DespawnAll();
            }
            else
            {
                for (int i = m_CardContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(m_CardContainer.GetChild(i).gameObject);
                }
            }

            // Mặc định Growth Rate là rỗng nếu không có dữ liệu
            SetGrowthRateUI(m_TmpVitGrowthRate, null);
            SetGrowthRateUI(m_TmpPowGrowthRate, null);
            SetGrowthRateUI(m_TmpAgiGrowthRate, null);
            SetGrowthRateUI(m_TmpIntGrowthRate, null);
            SetGrowthRateUI(m_TmpStaGrowthRate, null);
            SetGrowthRateUI(m_TmpDefGrowthRate, null);

            // Ép kiểu
            PlayerCharacterDataSO playerChar = baseChar.ConvertAs<PlayerCharacterDataSO>();
            if (playerChar == null)
            {
                Debug.LogWarning($"<color=orange>[Card Load Debug]</color> 2. Nhân vật {baseChar.CharacterId} không phải là PlayerCharacterDataSO. Dừng load thẻ.");
                return;
            }

            // --- LOAD GROWTH RATE ---
            SetGrowthRateUI(m_TmpVitGrowthRate, playerChar.GetModifierByStatType(EStatusType.Vitality));
            SetGrowthRateUI(m_TmpPowGrowthRate, playerChar.GetModifierByStatType(EStatusType.Power));
            SetGrowthRateUI(m_TmpAgiGrowthRate, playerChar.GetModifierByStatType(EStatusType.Agi));
            SetGrowthRateUI(m_TmpIntGrowthRate, playerChar.GetModifierByStatType(EStatusType.Intelligence));
            SetGrowthRateUI(m_TmpStaGrowthRate, playerChar.GetModifierByStatType(EStatusType.Stamina));
            SetGrowthRateUI(m_TmpDefGrowthRate, playerChar.GetModifierByStatType(EStatusType.Defense));

            // --- LOAD UNIQUE CARDS ---
            if (playerChar.UniqueCardId == null)
            {
                Debug.LogWarning($"<color=orange>[Card Load Debug]</color> 3. List UniqueCardId của {playerChar.CharacterId} bị NULL!");
                return;
            }

            if (playerChar.UniqueCardId.Count == 0)
            {
                Debug.LogWarning($"<color=orange>[Card Load Debug]</color> 3. List UniqueCardId của {playerChar.CharacterId} TRỐNG (Count = 0)!");
                return;
            }

            Debug.Log($"<color=cyan>[Card Load Debug]</color> 4. Nhân vật có {playerChar.UniqueCardId.Count} thẻ Unique. Đang tìm CardConfigSO...");

            CardConfigSO cardConfig = ContextManager.Singleton.GetDataSO<CardConfigSO>();
            if (cardConfig == null)
            {
                Debug.LogError("<color=red>[Card Load Debug]</color> LỖI NGHIÊM TRỌNG: Không tìm thấy CardConfigSO trong ContextManager! (Chưa được nạp vào game?)");
                return;
            }

            int loadedCount = 0;
            foreach (string cardId in playerChar.UniqueCardId)
            {
                Debug.Log($"<color=cyan>[Card Load Debug]</color> 5. Đang tìm thẻ có ID: '{cardId}'...");
                CardSO cardSO = cardConfig.GetCardById(cardId);

                if (cardSO == null)
                {
                    Debug.LogWarning($"<color=orange>[Card Load Debug]</color> LỖI: Không tìm thấy thẻ nào có ID '{cardId}' trong CardConfigSO. Kiểm tra lại việc nhập text!");
                    continue;
                }

                if (m_UICardPrefab == null)
                {
                    Debug.LogError("<color=red>[Card Load Debug]</color> LỖI: Chưa kéo m_UICardPrefab vào Inspector của màn hình Character Selection!");
                    return;
                }

                if (m_CardContainer == null)
                {
                    Debug.LogError("<color=red>[Card Load Debug]</color> LỖI: Chưa kéo m_CardContainer vào Inspector!");
                    return;
                }

                Debug.Log($"<color=cyan>[Card Load Debug]</color> 6. TÌm thấy thẻ '{cardSO.CardName}', tiến hành spawn UI...");

                // Spawn Prefab UI của Card qua PoolManager
                Transform cardTrans = null;
                if (PoolManager.Pools.ContainsKey(poolKey))
                {
                    cardTrans = PoolManager.Pools[poolKey].Spawn(m_UICardPrefab.transform, m_CardContainer);
                }
                else
                {
                    Debug.LogWarning($"<color=orange>[Card Load Debug]</color> PoolManager không chứa key '{poolKey}', dùng Instantiate mặc định...");
                }

                if (cardTrans == null)
                {
                    Debug.LogError("<color=red>[Card Load Debug]</color> LỖI: Không thể spawn card prefab ra Scene!");
                    continue;
                }

                UIUniqueCardElement cardUI = cardTrans.GetComponent<UIUniqueCardElement>();

                if (cardUI != null)
                {
                    string desc = cardSO.CardDescription;
                    cardUI.SetContent(cardSO.CardId, 
                                      LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, cardSO.CardName), 
                                      LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, cardSO.CardDescription), 
                                      cardSO.Icon
                                      );
                    cardUI.SetOnSelectCallback(OnUniqueCardClicked);
                    loadedCount++;
                    Debug.Log($"<color=green>[Card Load Debug]</color> 7. Load THÀNH CÔNG lên UI thẻ: {cardSO.CardName}");
                }
                else
                {
                    Debug.LogError("<color=red>[Card Load Debug]</color> LỖI: Prefab sinh ra KHÔNG GẮN script UIUniqueCardElement! Hãy mở prefab lên kiểm tra.");
                }
            }

            Debug.Log($"<color=cyan>[Card Load Debug]</color> 8. Hoàn tất. Tổng số thẻ đã hiện lên UI: {loadedCount}");
        }

        // Hàm xử lý khi người chơi bấm vào Card Đặc trưng
        private void OnUniqueCardClicked(string cardId, bool isSelected, Transform cardTransform)
        {
            Debug.Log($"Người chơi đã ấn vào thẻ Unique Card có ID: {cardId}");

            // GỌI POPUP LÊN
            CardSO cardSO = ContextManager.Singleton.GetDataSO<CardConfigSO>().GetCardById(cardId);
            if (cardSO != null)
            {
                UIManager.ShowFrame(GameConstants.FRAME_ID_CARD_DETAILS)
                 .AsFrame<UICardDetailFrame>()
                 .LoadData(cardSO);
            }
        }

        // Hàm hỗ trợ format text Growth Rate
        private void SetGrowthRateUI(TextMeshProUGUI tmpInfo, StatusModifierSO modSO)
        {
            if (tmpInfo == null) return;

            if (modSO == null)
            {
                tmpInfo.text = "";
                return;
            }

            string prefix = "";
            string valueStr = "";
            string suffix = "";
            Color textColor = Color.white; // Mặc định

            // 1. Phân loại theo toán tử (EOperator) để xác định Dấu, Hậu tố và Màu sắc
            switch (modSO.Operator)
            {
                case EOperator.PercentAdd:
                    prefix = "+";
                    suffix = "%";
                    valueStr = (modSO.Value * 100f).ToString("0"); // Ví dụ: 0.2 -> 20
                    textColor = Color.green; // Tăng -> Xanh
                    break;

                case EOperator.PercentSub:
                    prefix = "-";
                    suffix = "%";
                    valueStr = (modSO.Value * 100f).ToString("0");
                    textColor = Color.red;   // Giảm -> Đỏ
                    break;

                case EOperator.FlatAdd:
                    prefix = "+";
                    suffix = "";
                    valueStr = modSO.Value.ToString("0");
                    textColor = Color.green; // Tăng -> Xanh
                    break;

                case EOperator.FlatSub:
                    prefix = "-";
                    suffix = "";
                    valueStr = modSO.Value.ToString("0");
                    textColor = Color.red;   // Giảm -> Đỏ
                    break;

                case EOperator.Mul:
                    prefix = "x";
                    suffix = "";
                    valueStr = modSO.Value.ToString("0.##");
                    textColor = Color.yellow; // Nhân -> Vàng (có thể đổi)
                    break;

                case EOperator.Set:
                    prefix = "=";
                    suffix = "";
                    valueStr = modSO.Value.ToString("0");
                    textColor = Color.white;
                    break;

                default:
                    valueStr = modSO.Value.ToString("0.##");
                    break;
            }

            // 2. Gán text và màu
            tmpInfo.text = $"{prefix}{valueStr}{suffix}";
            tmpInfo.color = textColor;
        }

        private void ShowPrevChar()
        {
            _currentCharIndex--;
            if (_currentCharIndex < 0)
            {
                _currentCharIndex = _characterList.Count - 1;
            }

            BaseCharacterSO charData = _characterList[_currentCharIndex];
            SelectCharacter(charData.CharacterId);
        }

        private void ShowNextChar()
        {
            _currentCharIndex++;
            if (_currentCharIndex > _characterList.Count - 1)
            {
                _currentCharIndex = 0;
            }
            BaseCharacterSO charData = _characterList[_currentCharIndex];
            SelectCharacter(charData.CharacterId);
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);

            PlayerData playerData = _playerDAO.GetById(AuthManager.GetUserId());

            if (playerData != null && playerData.LegacyPoints > 0)
            {
                UIManager.ShowFrame(GameConstants.FRAME_ID_LEGACY_UPGRADE);
                return;
            }

            UIManager.ShowFrame(GameConstants.FRAME_ID_TITLE);
        }

        private void Next()
        {
            if (SessionController.CreateNewSession(_currentCharId, out string sessionId, out string error))
            {
                SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
            }
            else
            {
                Debug.LogError($"[UICharacterSelectionFrame.Next error]: {error}");
            }
        }
    }
}