using System;

namespace ElympicsLobbyPackage.Leaderboard
{
    public struct UserHighScore
    {
        public float Score { get; init; }
        public DateTimeOffset? EndedAt { get; init; }
    }
}
