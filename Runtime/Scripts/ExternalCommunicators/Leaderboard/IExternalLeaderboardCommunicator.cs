#nullable enable
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage.Leaderboard;

namespace ElympicsLobbyPackage.ExternalCommunication.Leaderboard
{
    public interface IExternalLeaderboardCommunicator
    {
        public UniTask<LeaderboardStatus> FetchLeaderboard(string? tournamentId, string? queueName, LeaderboardTimeScope timeScope, int pageNumber, int pageSize, LeaderboardRequestType leaderboardRequestType);
        public UniTask<UserHighScore> FetchUserHighScore(string? tournamentId, string? queueName, LeaderboardTimeScope timeScope, int pageNumber, int pageSize);
    }
}
