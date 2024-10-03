using System;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;

namespace ElympicsLobbyPackage.WebMessages.Models
{
    [Serializable]
    public class TournamentUpdatedMessage
    {
        public TournamentDataDto tournamentData;
    }
}
