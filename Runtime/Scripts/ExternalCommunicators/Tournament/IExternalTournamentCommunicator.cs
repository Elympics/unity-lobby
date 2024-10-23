using System;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Tournament;

namespace ElympicsLobbyPackage.ExternalCommunication.Tournament
{
    public interface IExternalTournamentCommunicator
    {
        event Action<TournamentInfo> TournamentUpdated;
        UniTask<CanPlayTournamentResponse> CanPlayTournament();

    }
}
