namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Addons.Localization;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICharacterSelectionFrame : GameUIFrame
    {
        [SerializeField] private Transform m_UICharacterPrefab;
        [SerializeField] private Transform m_CharacterContainer;

        [SerializeField] private Image m_CharacterImg;
        [SerializeField] private TextMeshProUGUI m_CharacterNameTmp;
        [SerializeField] private TextMeshProUGUI m_CharacterDetailsTmp;

        [SerializeField] private Button m_PrevCharBtn;
        [SerializeField] private Button m_NextCharBtn;

        [SerializeField] private Button m_BackBtn;
        [SerializeField] private Button m_NextBtn;

        private CharacterConfigSO _characterConfig;
        private LocalizationManager _localizeManager;
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
        private LocalizationManager LocalizeManager
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
        private GameSessionController SessionController
        {
            get
            {
                if (_sessionController == null)
                {
                    bool success = ContextManager.Singleton.TryResolveSceneContext<GameSessionController>(out _sessionController);
                }
                return _sessionController;
            }
        }

        private int _currentCharIndex;
        private string _currentCharId;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_PrevCharBtn.onClick.AddListener(ShowPrevChar);
            m_NextCharBtn.onClick.AddListener(ShowNextChar);
            m_BackBtn.onClick.AddListener(Back);
            m_NextBtn.onClick.AddListener(Next);
            LoadCharacters();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_PrevCharBtn.onClick.RemoveListener(ShowPrevChar);
            m_NextCharBtn.onClick.RemoveListener(ShowNextChar);

            m_BackBtn.onClick.RemoveListener(Back);
            m_NextBtn.onClick.RemoveListener(Next);

            ClearAllChars();
        }

        private void LoadCharacters()
        {
            foreach (var characterData in CharacterConfig.GetCharactersByType(ECharacterType.Playable))
            {
                Transform charUITrans = PoolManager.Pools["UICharacter"].Spawn(m_UICharacterPrefab, m_CharacterContainer);
                UICharacterElement characterUI = charUITrans.GetComponent<UICharacterElement>();
                if (characterUI != null)
                {
                    characterUI.SetSelectCallback(SelectCharacter).SetContent(characterData.CharacterId, characterData.Thumbnail);
                }
            }
            BaseCharacterSO firstChar = CharacterConfig.Characters[0];
            SelectCharacter(firstChar.CharacterId);
        }
        private void ClearAllChars()
        {
            if (PoolManager.Pools["UICharacter"].Count > 0)
            {
                PoolManager.Pools["UICharacter"].DespawnAll();
            }
        }

        private void SelectCharacter(string characterId)
        {
            if (string.IsNullOrEmpty(characterId))
            {
                // Display error here.
                return;
            }

            // Display character details here.
            Debug.Log($"Select {characterId}");
            _currentCharId = characterId;
            BaseCharacterSO characterSO = CharacterConfig.GetCharacterById(characterId);
            string charName = characterSO.CharacterName;
            string desc = LocalizeManager.GetText("CharacterDescs", characterSO.CharacterDescription);

            m_CharacterImg.sprite = characterSO.FullBodyImg;
            m_CharacterNameTmp.text = charName;
            m_CharacterDetailsTmp.text = desc;
        }

        private void ShowPrevChar()
        {
            _currentCharIndex--;
            if (_currentCharIndex < 0)
            {
                _currentCharIndex = CharacterConfig.Characters.Length - 1;
            }

            BaseCharacterSO charData = CharacterConfig.Characters[_currentCharIndex];
            SelectCharacter(charData.CharacterId);
        }

        private void ShowNextChar()
        {
            _currentCharIndex++;
            if (_currentCharIndex > CharacterConfig.Characters.Length - 1)
            {
                _currentCharIndex = 0;
            }
            BaseCharacterSO charData = CharacterConfig.Characters[_currentCharIndex];
            SelectCharacter(charData.CharacterId);
        }

        private void Back()
        {
            UIManager.HideFrame(GameConstants.FRAME_ID_CHAR_SELECT);
            UIManager.ShowFrame(GameConstants.FRAME_ID_TITLE);
        }

        private void Next()
        {
            // Create new session here.
            if (SessionController.CreateNewSession(_currentCharId, out string sessionId, out string error))
            {
                PlayerPrefs.SetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID, sessionId);

                SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
            }
            else
            {
                Debug.LogError($"Error: {error}");
            }
        }
    }
}