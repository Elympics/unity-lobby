using System;
using Elympics;

namespace ElympicsLobbyPackage.ExternalCommunication.Leaderboard.Models
{
    [Serializable]
    internal class LeaderboardRequest
    {
        public string tournamentId;
        public string queueName;
        public LeaderboardTimeScopeType timeScopeType;
        public string dateFrom;
        public string dateTo;
        public string fetchType;
        public int pageNumber;
        public int pageSize;
    }
}
