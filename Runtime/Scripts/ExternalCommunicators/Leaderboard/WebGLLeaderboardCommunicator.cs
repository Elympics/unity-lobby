using System;
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.ExternalCommunication.Leaderboard.Models;
using ElympicsLobbyPackage.Leaderboard;
using LeaderboardResponse = ElympicsLobbyPackage.ExternalCommunication.Leaderboard.Models.LeaderboardResponse;

#nullable enable
namespace ElympicsLobbyPackage.ExternalCommunication.Leaderboard
{
    internal class WebGLLeaderboardCommunicator : IExternalLeaderboardCommunicator
    {
        private readonly JsCommunicator _jsCommunicator;
        public WebGLLeaderboardCommunicator(JsCommunicator jsCommunicator) => _jsCommunicator = jsCommunicator;

        public async UniTask<LeaderboardStatus> FetchLeaderboard(
            string? tournamentId,
            string? queueName,
            LeaderboardTimeScope timeScope,
            int pageNumber,
            int pageSize,
            LeaderboardRequestType leaderboardRequestType)
        {
            var request = new LeaderboardRequest
            {
                tournamentId = tournamentId,
                queueName = queueName,
                timeScopeType = timeScope.LeaderboardTimeScopeType,
                pageNumber = pageNumber,
                pageSize = pageSize,
                fetchType = "Max",
                dateFrom = timeScope.LeaderboardTimeScopeType == LeaderboardTimeScopeType.Custom ? timeScope.DateFrom.ToString("o") : string.Empty,
                dateTo = timeScope.LeaderboardTimeScopeType == LeaderboardTimeScopeType.Custom ? timeScope.DateTo.ToString("o") : string.Empty,
            };
            var response = leaderboardRequestType switch
            {
                LeaderboardRequestType.Regular => await _jsCommunicator.SendRequestMessage<LeaderboardRequest, LeaderboardResponse>(ReturnEventTypes.GetLeaderboard, request),
                LeaderboardRequestType.UserCentered => await _jsCommunicator.SendRequestMessage<LeaderboardRequest, LeaderboardResponse>(ReturnEventTypes.GetLeaderboardUserCentered, request),
                _ => throw new ArgumentOutOfRangeException(nameof(leaderboardRequestType), leaderboardRequestType, null)
            };
            return response.MapToLeaderboardStatus();
        }
        public async UniTask<UserHighScore> FetchUserHighScore(
            string? tournamentId,
            string? queueName,
            LeaderboardTimeScope timeScope,
            int pageNumber,
            int pageSize)
        {
            var request = new LeaderboardRequest
            {
                tournamentId = tournamentId,
                queueName = queueName,
                timeScopeType = timeScope.LeaderboardTimeScopeType,
                pageNumber = pageNumber,
                pageSize = pageSize,
                fetchType = "Max",
                dateFrom = timeScope.LeaderboardTimeScopeType == LeaderboardTimeScopeType.Custom ? timeScope.DateFrom.ToString("o") : string.Empty,
                dateTo = timeScope.LeaderboardTimeScopeType == LeaderboardTimeScopeType.Custom ? timeScope.DateTo.ToString("o") : string.Empty,
            };
            var response = await _jsCommunicator.SendRequestMessage<LeaderboardRequest, UserHighScoreResponse>(ReturnEventTypes.GetLeaderboardUserHighScore, request);
            return response.MapToUserHighScore();
        }
    }
}
