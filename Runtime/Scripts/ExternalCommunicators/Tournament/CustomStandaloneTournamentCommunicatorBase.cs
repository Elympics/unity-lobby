using System;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Tournament;
using UnityEngine;

namespace ElympicsLobbyPackage.ExternalCommunication.Tournament
{
    public abstract class CustomStandaloneTournamentCommunicatorBase : MonoBehaviour, IExternalTournamentCommunicator
    {
        public event Action<TournamentInfo> TournamentUpdated;
        public abstract UniTask<CanPlayTournamentResponse> CanPlayTournament();
    }
}
