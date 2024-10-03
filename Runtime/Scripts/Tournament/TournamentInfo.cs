using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ElympicsLobbyPackage.Tournament
{
    public readonly struct TournamentInfo
    {
        public int LeaderboardCapacity { get; init; }
        public string Name { get; init; }
        public Guid OwnerId { get; init; }
        internal TournamentState State { get; init; }
        public DateTimeOffset CreateDate { get; init; }
        public DateTimeOffset StartDate { get; init; }
        public DateTimeOffset EndDate { get; init; }
        public List<Guid> Participants { get; init; }
    }
}
