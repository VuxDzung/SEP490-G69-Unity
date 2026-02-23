namespace SEP490G69.GameSessions
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UICharacterSelectionFrame : GameUIFrame
    {
        [SerializeField] private Transform m_UICharacterPrefab;
        [SerializeField] private Transform m_CharacterContainer;
        [SerializeField] private Button m_SelectBtn;

        private CharacterConfigSO _characterConfig;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (_characterConfig == null) _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
            m_SelectBtn.onClick.AddListener(ChooseCharacter);
            LoadCharacters();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_SelectBtn.onClick.RemoveListener(ChooseCharacter);
            ClearAllChars();
        }

        private void LoadCharacters()
        {
            foreach (var characterData in _characterConfig.Characters)
            {
                Transform charUITrans = PoolManager.Pools["UICharacter"].Spawn(m_UICharacterPrefab, m_CharacterContainer);
                UICharacterElement characterUI = charUITrans.GetComponent<UICharacterElement>();
                if (characterUI != null)
                {
                    characterUI.SetSelectCallback(SelectCharacter).SetContent(characterData.CharacterID, characterData.Avatar);
                }
            }
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
        }

        private void ChooseCharacter()
        {

        }
    }
}