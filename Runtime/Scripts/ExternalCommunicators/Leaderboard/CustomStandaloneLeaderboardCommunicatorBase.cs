using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage.Leaderboard;
using UnityEngine;
using LeaderboardResponse = ElympicsLobbyPackage.ExternalCommunication.Leaderboard.Models.LeaderboardResponse;

namespace ElympicsLobbyPackage.ExternalCommunication.Leaderboard
{
    public abstract class CustomStandaloneLeaderboardCommunicatorBase : MonoBehaviour, IExternalLeaderboardCommunicator
    {
        public abstract UniTask<LeaderboardStatus> FetchLeaderboard(
            string tournamentId,
            string queueName,
            LeaderboardTimeScope timeScope,
            int pageNumber,
            int pageSize,
            LeaderboardRequestType leaderboardRequestType);
        public UniTask<UserHighScore> FetchUserHighScore(string tournamentId, string queueName, LeaderboardTimeScope timeScope, int pageNumber, int pageSize) => throw new System.NotImplementedException();
    }
}
