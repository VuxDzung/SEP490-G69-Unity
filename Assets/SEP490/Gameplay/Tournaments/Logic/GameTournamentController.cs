namespace SEP490G69.Tournament
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using SEP490G69.GameSessions;

    public class GameTournamentController : MonoBehaviour, ISceneContext
    {
        [Header("Configurations")]
        [SerializeField] private TournamentConfigSO m_TournamentConfig;
        [SerializeField] private CharacterConfigSO m_CharacterConfig;
        [Header("Testing")]
        [SerializeField] private bool m_IsTesting;
        [SerializeField] private string m_TournamentId;

        // DAOs
        private GameSessionDAO _sessionDAO;
        private PlayerCharacterRepository _characterRepo;

        // Runtime Data
        private List<TournamentParticipant> m_CurrentParticipants = new List<TournamentParticipant>();
        private List<TournamentParticipant> m_CurrentRoundParticipants = new List<TournamentParticipant>();
        private int m_CurrentRoundIndex = 0;
        private TournamentSO m_CurrentTournamentData;
        private UITournamentBracketScreen _bracketFrame;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
            LoadDAOs();
        }

        private void Start()
        {
            if (m_IsTesting)
            {
                LoadTournamentData(m_TournamentId);
            }
        }

        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void LoadDAOs()
        {
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
            m_CurrentRoundParticipants.Clear();
            m_CurrentRoundParticipants.AddRange(m_CurrentParticipants.ToArray());
            m_CurrentRoundIndex = 0;

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
            EnemySO enemySO = m_CharacterConfig.GetCharacterById(enemyId).ConvertAs<EnemySO>(); // Giả định bạn có hàm này
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
            // Display bracket frame.
            _bracketFrame = GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_TOURNAMENT_BRACKET).AsFrame<UITournamentBracketScreen>();

            // Chuyển data sang UI
            _bracketFrame.SetupTournament(m_CurrentTournamentData.Name, m_CurrentParticipants, this);
        }

        // ==========================================
        // TOURNAMENT PROGRESSION LOGIC
        // ==========================================

        public void RequestProgressTournament()
        {
            if (m_CurrentRoundParticipants == null || m_CurrentRoundParticipants.Count <= 1)
            {
                Debug.LogError("m_CurrentRoundParticipants is null or m_CurrentRoundParticipants.Count <= 1");
                return;
            }

            List<TournamentParticipant> winners = new List<TournamentParticipant>();

            for (int i = 0; i < m_CurrentRoundParticipants.Count; i += 2)
            {
                TournamentParticipant p1 = m_CurrentRoundParticipants[i];
                TournamentParticipant p2 = m_CurrentRoundParticipants[i + 1];

                TournamentParticipant winner = ResolveMatch(p1, p2);
                winners.Add(winner);
            }
            m_CurrentRoundParticipants.Clear();
            m_CurrentRoundParticipants = winners;
            m_CurrentRoundIndex++;

            UpdateBracketUI();

            // Tournament finished
            if (m_CurrentRoundParticipants.Count == 1)
            {
                OnTournamentFinished(m_CurrentRoundParticipants[0]);
            }
        }

        private TournamentParticipant ResolveMatch(TournamentParticipant p1, TournamentParticipant p2)
        {
            Debug.Log($"Enter Match: {p1.Name} vs {p2.Name}");

            // Player match
            //if (p1.IsPlayer || p2.IsPlayer)
            //{
            //    // TODO: Load Combat Scene
            //    // Hiện tại mock player win

            //    if (p1.IsPlayer)
            //    {
            //        p2.IsEliminated = true;
            //        return p1;
            //    }
            //    else
            //    {
            //        p1.IsEliminated = true;
            //        return p2;
            //    }
            //}

            // NPC vs NPC
            if (p1.TotalStats >= p2.TotalStats)
            {
                p2.IsEliminated = true;
                return p1;
            }
            else
            {
                p1.IsEliminated = true;
                return p2;
            }
        }
        private void UpdateBracketUI()
        {
            switch (m_CurrentRoundIndex)
            {
                case 1:
                    _bracketFrame.UpdateQuarterFinals(m_CurrentRoundParticipants);
                    break;

                case 2:
                    _bracketFrame.UpdateSemiFinals(m_CurrentRoundParticipants);
                    break;

                case 3:
                    _bracketFrame.UpdateFinal(m_CurrentRoundParticipants);
                    break;
            }
        }
        private void OnTournamentFinished(TournamentParticipant champion)
        {
            Debug.Log($"Tournament Champion: {champion.Name}");

            _bracketFrame.ShowChampion(champion);

            if (champion.IsPlayer)
            {
                Debug.Log("Player wins tournament!");
                // TODO: Give rewards
            }
        }
    }
}