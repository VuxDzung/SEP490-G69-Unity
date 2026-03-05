namespace SEP490G69.Tournament
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using SEP490G69.GameSessions; // Namespace chứa DAO

    public class GameTournamentController : MonoBehaviour, ISceneContext
    {
        [Header("Configurations")]
        [SerializeField] private TournamentConfigSO m_TournamentConfig;
        [SerializeField] private CharacterConfigSO m_CharacterConfig;

        // Bạn cần tạo EnemyConfigSO chứa List<EnemySO> tương tự CharacterConfigSO
        [SerializeField] private EnemyConfigSO m_EnemyConfig;

        // DAOs
        private GameSessionDAO _sessionDAO;
        private PlayerCharacterRepository _characterRepo;

        // Runtime Data
        private List<TournamentParticipant> m_CurrentParticipants = new List<TournamentParticipant>();
        private TournamentSO m_CurrentTournamentData;
        private UITournamentBracketScreen _uiBracket;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
            LoadDAOs();
        }

        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void LoadDAOs()
        {
            // Khởi tạo database như bên TrainingController
            _sessionDAO = new GameSessionDAO(LocalDBInitiator.GetDatabase());
            _characterRepo = new PlayerCharacterRepository(LocalDBInitiator.GetDatabase());
        }

        public void LoadTournamentData(string tournamentId)
        {
            m_CurrentTournamentData = m_TournamentConfig.GetTournamentById(tournamentId);
            if (m_CurrentTournamentData == null) return;

            m_CurrentParticipants.Clear();

            // 1. Load Player
            TournamentParticipant player = GetPlayerParticipantData();
            if (player != null) m_CurrentParticipants.Add(player);

            // 2. Load Enemies
            int npcCount = Mathf.Min(7, m_CurrentTournamentData.EnemyIds.Length);
            for (int i = 0; i < npcCount; i++)
            {
                string enemyId = m_CurrentTournamentData.EnemyIds[i];
                m_CurrentParticipants.Add(GetNPCParticipantData(enemyId));
            }

            // 3. Shuffle
            ShuffleParticipants(m_CurrentParticipants);

            // 4. Open UI
            OpenTournamentBracketUI();
        }

        private TournamentParticipant GetPlayerParticipantData()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId)) return null;

            PlayerTrainingSession sessionData = _sessionDAO.GetSession(sessionId);
            if (sessionData == null) return null;

            SessionCharacterData characterData = _characterRepo.GetCharacterData(sessionId, sessionData.CharacterId);
            BaseCharacterSO characterSO = m_CharacterConfig.GetCharacterById(sessionData.CharacterId);

            CharacterDataHolder holder = new CharacterDataHolder.Builder()
                .WithCharacterData(characterData)
                .WithCharacterSO(characterSO)
                .Build();

            // Lấy tổng chỉ số của Player
            float totalStats = holder.GetVIT() + holder.GetPower() + holder.GetAgi() + holder.GetINT() + holder.GetStamina();

            return new TournamentParticipant
            {
                Id = holder.GetRuntimeId(),
                Name = holder.GetCharacterName(),
                Avatar = holder.GetAvatar(),
                IsPlayer = true,
                TotalStats = totalStats,
                IsEliminated = false
            };
        }

        private TournamentParticipant GetNPCParticipantData(string enemyId)
        {
            EnemySO enemySO = m_EnemyConfig.GetEnemyById(enemyId); // Giả định bạn có hàm này
            if (enemySO == null) return new TournamentParticipant();

            return new TournamentParticipant
            {
                Id = enemySO.CharacterId, // Kế thừa từ BaseCharacterSO
                Name = enemySO.CharacterName,
                Avatar = enemySO.Thumbnail,
                IsPlayer = false,
                TotalStats = enemySO.TotalStats,
                IsEliminated = false
            };
        }

        private void ShuffleParticipants(List<TournamentParticipant> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TournamentParticipant temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        private void OpenTournamentBracketUI()
        {
            // Hiển thị Frame (Giả định bạn dùng GameUIManager)
            _uiBracket = GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_TOURNAMENT_BRACKET).AsFrame<UITournamentBracketScreen>();

            // Chuyển data sang UI
            _uiBracket.SetupTournament(m_CurrentTournamentData.Name, m_CurrentParticipants, this);
        }

        // ==========================================
        // TOURNAMENT PROGRESSION LOGIC
        // ==========================================

        public void RequestProgressTournament()
        {
            // [Inference] Hàm này được gọi khi Player bấm nút "Continue/Next Match" trên UI
            // Ở đây tôi làm logic ví dụ cho Vòng 1 (Tứ kết - 8 người -> 4 trận)

            List<TournamentParticipant> nextRoundWinners = new List<TournamentParticipant>();

            for (int i = 0; i < m_CurrentParticipants.Count; i += 2)
            {
                var p1 = m_CurrentParticipants[i];
                var p2 = m_CurrentParticipants[i + 1];

                // Nếu có Player tham gia trận này -> Chuyển sang Scene Combat
                if (p1.IsPlayer || p2.IsPlayer)
                {
                    Debug.Log($"Vào trận đấu: {p1.Name} vs {p2.Name}");
                    // TODO: Chuyển Scene Combat. Sau khi combat xong cập nhật kết quả.
                    // Tạm thời giả lập Player luôn thắng để test UI
                    p2.IsEliminated = true;
                    nextRoundWinners.Add(p1);
                }
                else
                {
                    // Auto Resolve cho NPC vs NPC (Dựa theo quy tắc BR-57)
                    if (p1.TotalStats >= p2.TotalStats)
                    {
                        p2.IsEliminated = true;
                        nextRoundWinners.Add(p1);
                    }
                    else
                    {
                        p1.IsEliminated = true;
                        nextRoundWinners.Add(p2);
                    }
                }
            }

            // Gọi UI cập nhật lại Bracket vòng tiếp theo
            _uiBracket.UpdateQuarterFinals(nextRoundWinners);
        }
    }
}