using System;
using System.Globalization;
using Elympics;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;

namespace ElympicsLobbyPackage
{
    public static class TournamentExt
    {
        public static TournamentInfo ToTournamentInfo(this TournamentDataDto dto)
        {
            var startDate = DateTimeOffset.Parse(dto.startDate, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal);
            var endDate = DateTimeOffset.Parse(dto.endDate, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal);
            var createdDate = DateTimeOffset.Parse(dto.createDate, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal);

            var ownerId = Guid.Parse(dto.ownerId);
            var state = (TournamentState)dto.state;

            return new TournamentInfo
            {
                LeaderboardCapacity = dto.leaderboardCapacity,
                Name = dto.name,
                OwnerId = ownerId,
                State = state,
                CreateDate = createdDate,
                StartDate = startDate,
                EndDate = endDate,
                Participants = dto.participants
            };
        }
    }
}
