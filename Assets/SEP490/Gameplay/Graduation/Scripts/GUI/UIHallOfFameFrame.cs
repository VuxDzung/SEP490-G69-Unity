namespace SEP490G69.Graduation
{
    using SEP490G69.GameSessions;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIHallOfFameFrame : GameUIFrame
    {
        [SerializeField] private Transform m_Container;
        [SerializeField] private Transform m_UIPrefab;
        [SerializeField] private GameObject m_NoRecordGO;
        [SerializeField] private Transform m_CharContainer;
        [SerializeField] private Transform m_CharPrefab;

        private List<EndGameRecordData> playerRecords = new List<EndGameRecordData>();

        private CharacterConfigSO _characterConfig;
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
        private SceneCharacterLoader _characterLoader;
        private SceneCharacterLoader CharacterLoader
        {
            get
            {
                if (_characterLoader == null)
                {
                    _characterLoader = ContextManager.Singleton.GetSceneContext<SceneCharacterLoader>();
                }
                return _characterLoader;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            ClearChildren("UICharacter");
            ClearChildren("UICharacterRecord");
        }

        public void LoadRecords(List<EndGameRecordData> records)
        {
            if (records == null || records.Count == 0)
                return;
            playerRecords = records;
            // ===== Clear old UI =====
            ClearChildren("UICharacter");
            ClearChildren("UICharacterRecord");



            List<string> characterIdList = records.Select(record => record.RawCharacterId).ToList();

            foreach (BaseCharacterSO characterSO in CharacterConfig.GetCharactersByType(ECharacterType.Playable))
            {
                // ===== Spawn Character Button =====
                GameObject charGO = PoolManager.Pools["UICharacter"].Spawn(m_CharPrefab, m_CharContainer).gameObject;

                Button btn = charGO.GetComponent<Button>(); // hoặc Button

                // TODO: set name/icon nếu có
                // charGO.GetComponentInChildren<TextMeshProUGUI>().text = playerId;
                UICharacterElement characterUI = charGO.GetComponent<UICharacterElement>();
                if (characterUI != null)
                {

                    characterUI.SetSelectCallback(SelectCharacter);
                    characterUI.SetContent(characterSO.CharacterId, characterSO.Thumbnail);
                }
            }
        }

        private void SelectCharacter(string characterId)
        {
            BaseCharacterSO characterSO = CharacterConfig.GetCharacterById(characterId);
            if (characterSO == null) return;    
            if (CharacterLoader != null)
            {
                CharacterLoader.LoadPlayerCharacter(characterSO);
            }
            List<EndGameRecordData> records = playerRecords.Where(record => record.RawCharacterId == characterId).OrderByDescending(record => record.Rating).ToList();
            LoadCharacterRecords(records);
        }

        private void LoadCharacterRecords(List<EndGameRecordData> records)
        {
            ClearChildren("UICharacterRecord");

            if (records.Count > 0)
            {
                m_NoRecordGO.SetActive(false);
                for (int i = 0; i < records.Count; i++)
                {
                    var record = records[i];

                    GameObject go = PoolManager.Pools["UICharacterRecord"].Spawn(m_UIPrefab, m_Container).gameObject;

                    UIEndGameRecordElement element = go.GetComponent<UIEndGameRecordElement>();

                    element.Spawn();

                    // Nếu bạn có timestamp → dùng, chưa có thì fake tạm
                    System.DateTime date = System.DateTime.Now;
                    string order = GetOrdinal(i + 1);

                    element.SetContent(
                        order,
                        record.Rating,
                        record.Title,
                        record.EndingType,
                        date
                    );
                }
            }
            else
            {
                m_NoRecordGO.SetActive(true);
            }
        }

        private string GetOrdinal(int number)
        {
            int abs = Mathf.Abs(number);
            int lastTwo = abs % 100;

            // Special case: 11, 12, 13
            if (lastTwo >= 11 && lastTwo <= 13)
                return number + "th";

            switch (abs % 10)
            {
                case 1: return number + "st";
                case 2: return number + "nd";
                case 3: return number + "rd";
                default: return number + "th";
            }
        }

        private void ClearChildren(string poolName)
        {
            PoolManager.Pools[poolName].DespawnAll();
        }
    }
}