using System;
using ElympicsLobbyPackage.Tournament;

namespace ElympicsLobbyPackage.ExternalCommunication.Tournament
{
    public interface IExternalTournamentCommunicator
    {
        event Action<TournamentInfo> TournamentUpdated;
    }
}
