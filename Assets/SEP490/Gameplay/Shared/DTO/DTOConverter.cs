namespace SEP490G69.GameSessions
{
    using System.Collections.Generic;
    using SEP490G69.Economy;
    using SEP490G69.Tournament;

    public class DTOConverter 
    {
        public static SessionPlayerDeckDTO FromDB2DTO(SessionPlayerDeck deck)
        {
            if (deck == null)
            {
                return null;
            }

            return new SessionPlayerDeckDTO
            {
                SessionId = deck.SessionId,
                CardIds = deck.CardIds,
            };
        }


        public static List<SessionTrainingExerciseDTO> FromDBList2DTO(List<SessionTrainingExercise> exercises)
        {
            if (exercises == null || exercises.Count == 0)
            {
                return new List<SessionTrainingExerciseDTO>();
            }

            List<SessionTrainingExerciseDTO> dtoList = new List<SessionTrainingExerciseDTO>();  
            foreach (var exercise in exercises)
            {
                SessionTrainingExerciseDTO dto = new SessionTrainingExerciseDTO
                {
                    Id = exercise.Id,
                    ExerciseId = exercise.Id,
                    SessionId = exercise.SessionId,
                    Level = exercise.Level,
                };
                if (dto != null) dtoList.Add(dto);
            }
            return dtoList;
        }


        public static List<SessionCardDataDTO> FromDBList2DTO(List<SessionCardData> cards)
        {
            if (cards == null || cards.Count == 0)
            {
                return new List<SessionCardDataDTO>();
            }

            List<SessionCardDataDTO> dtoList = new List<SessionCardDataDTO>();
            foreach (var card in cards)
            {
                SessionCardDataDTO dto = FromDB2DTO(card);
                if (dto != null) dtoList.Add(dto);
            }
            return dtoList;
        }
        public static SessionCardDataDTO FromDB2DTO(SessionCardData cardDB)
        {
            if (cardDB == null)
            {
                return null;
            }
            return new SessionCardDataDTO
            {
                SessionCardId = cardDB.SessionCardId,
                SessionId = cardDB.SessionId,
                RawCardId = cardDB.RawCardId,
                ObtainedAmount = cardDB.ObtainedAmount,
            };
        }


        public static PlayerTrainingSessionDTO FromDB2DTO(PlayerTrainingSession session)
        {
            if (session == null)
            {
                return null;
            }
            return new PlayerTrainingSessionDTO
            {
                SessionId = session.SessionId,
                PlayerId = session.PlayerId,
                RawCharacterId = session.RawCharacterId,
                CurrentGoldAmount = session.CurrentGoldAmount,
                CurrentWeek = session.CurrentWeek,
                ActiveTournamentId = session.ActiveTournamentId,
            };
        }

        public static SessionCharacterDataDTO FromDB2DTO(SessionCharacterData character)
        {
            if (character == null)
            {
                return null;
            }

            return new SessionCharacterDataDTO
            {
                Id = character.Id,
                SessionId = character.SessionId,
                RawCharacterId = character.RawCharacterId,
                CurrentMaxVitality = character.CurrentMaxVitality,
                CurrentPower = character.CurrentPower,
                CurrentIntelligence = character.CurrentIntelligence,
                CurrentStamina = character.CurrentStamina,
                CurrentDef = character.CurrentDef,
                CurrentAgi = character.CurrentAgi,
                CurrentEnergy = character.CurrentEnergy,
                CurrentMood = character.CurrentMood,
                CurrentRP = character.CurrentRP,
            };
        }


        public static List<ItemDataDTO> FromDBList2DTO(List<ItemData> items)
        {
            if (items == null || items.Count == 0)
            {
                return new List<ItemDataDTO>();
            }

            List<ItemDataDTO> dtoList = new List<ItemDataDTO>();
            foreach (var item in items)
            {
                ItemDataDTO dto = FromDB2DTO(item);
                if (dto != null) dtoList.Add(dto);
            }
            return dtoList;
        }
        public static ItemDataDTO FromDB2DTO(ItemData item)
        {
            if (item == null)
            {
                return null;
            }

            return new ItemDataDTO
            {
                SessionItemId = item.SessionItemId,
                RawItemId = item.RawItemId,
                SessionId = item.SessionId,
                RemainAmount = item.RemainAmount,
            };
        }


        public static List<ShopItemDataDTO> FromDBList2DTO(List<ShopItemData> items)
        {
            if (items == null || items.Count == 0)
            {
                return new List<ShopItemDataDTO>();
            }

            List<ShopItemDataDTO> dtoList = new List<ShopItemDataDTO>();
            foreach (var item in items)
            {
                ShopItemDataDTO dto = FromDB2DTO(item);
                if (dto != null) dtoList.Add(dto);
            }
            return dtoList;
        }
        public static ShopItemDataDTO FromDB2DTO(ShopItemData item)
        {
            if (item == null)
            {
                return null;
            }
            return new ShopItemDataDTO
            {
                SessionItemId = item.SessionItemId,
                RawItemId = item.RawItemId,
                SessionId = item.SessionId,
                RemainAmount = item.RemainAmount,
            };
        }

        public static List<TournamentProgressDataDTO> FromDBList2DTO(List<TournamentProgressData> tournaments)
        {
            if (tournaments == null || tournaments.Count == 0)
            {
                return new List<TournamentProgressDataDTO>();
            }

            List<TournamentProgressDataDTO> dtoList = new List<TournamentProgressDataDTO>();
            foreach (var item in tournaments)
            {
                TournamentProgressDataDTO dto = FromDB2DTO(item);
                if (dto != null) dtoList.Add(dto);
            }
            return dtoList;
        }
        public static TournamentProgressDataDTO FromDB2DTO(TournamentProgressData tournament)
        {
            if (tournament == null)
            {
                return null;
            }
            return new TournamentProgressDataDTO
            {
                Id = tournament.Id,
                SessionId = tournament.SessionId,
                RawTournamentId = tournament.RawTournamentId,
                CurrentRoundIndex = tournament.CurrentRoundIndex,
                CurrentRoundParticipants = tournament.CurrentRoundParticipants,
                Participants = tournament.Participants,
                SemiFinalParticipants = tournament.SemiFinalParticipants,
                FinalParticipants = tournament.FinalParticipants,
                WaitingForPlayerBattle = tournament.WaitingForPlayerBattle,
                IsBattleFinished = tournament.IsBattleFinished,
                IsPlayerWon = tournament.IsPlayerWon,
                PendingEnemyId = tournament.PendingEnemyId,
            };
        }
    }
}