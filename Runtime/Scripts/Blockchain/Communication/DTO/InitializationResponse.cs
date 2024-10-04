#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
	[Serializable]
	internal class InitializationResponse
	{
		public AuthDataRaw authData;
		public string? error;
		public string device = null!;
		public string environment = null!;
		public int capabilities;
        public string closestRegion = null!;
        public TournamentDataDto tournamentData = null!;
    }

	[Serializable]
	internal struct AuthDataRaw
	{
		public string jwt;
		public string userId;
		public string nickname;
	}

    [Serializable]
    public class TournamentDataDto
    {
        public int leaderboardCapacity;
        public string name = null!;
        public string ownerId = null!;
        public int state;
        public string createDate = null!;
        public string startDate = null!;
        public string endDate = null!;
        public List<string> participants = null!;
    }
}
