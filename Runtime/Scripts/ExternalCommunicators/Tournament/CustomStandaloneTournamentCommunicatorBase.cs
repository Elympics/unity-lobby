using System;
using ElympicsLobbyPackage.Tournament;
using UnityEngine;

namespace ElympicsLobbyPackage.ExternalCommunication.Tournament
{
    public class CustomStandaloneTournamentCommunicatorBase : MonoBehaviour, IExternalTournamentCommunicator
    {
        public event Action<TournamentInfo> TournamentUpdated;
    }
}
