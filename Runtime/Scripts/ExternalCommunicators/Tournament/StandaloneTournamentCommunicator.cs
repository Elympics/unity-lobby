using System;
using ElympicsLobbyPackage.Blockchain.Communication;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.ExternalCommunication;
using ElympicsLobbyPackage.ExternalCommunication.Tournament;
using ElympicsLobbyPackage.Tournament;
using ElympicsLobbyPackage.Tournament.Util;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    public class StandaloneTournamentCommunicator : IExternalTournamentCommunicator, IWebMessageReceiver
    {
        private readonly StandaloneExternalTournamentConfig _config;
        private readonly JsCommunicator _jsCommunicator;
        public StandaloneTournamentCommunicator(StandaloneExternalTournamentConfig config) => _config = config;

        internal StandaloneTournamentCommunicator(StandaloneExternalTournamentConfig config, JsCommunicator jsCommunicator)
        {
            _config = config;
            jsCommunicator.RegisterIWebEventReceiver(this, Blockchain.Communication.WebMessages.TournamentUpdated);
        }
        public event Action<TournamentInfo> TournamentUpdated;
        public UniTask<CanPlayTournamentResponse> CanPlayTournament() => UniTask.FromResult(new CanPlayTournamentResponse()
        {
            statusCode = _config.CanPlayStatusCode,
            message = _config.CanPlayMessage
        });
        public void OnWebMessage(WebMessageObject message)
        {
            if (string.Equals(message.type, Blockchain.Communication.WebMessages.TournamentUpdated) is false)
                throw new Exception($"{nameof(WebGLTournamentCommunicator)} can handle only {Blockchain.Communication.WebMessages.TournamentUpdated} event type.");

            var newTournamentData = JsonUtility.FromJson<TournamentDataDto>(message.message);
            var tournamentInfo = newTournamentData?.ToTournamentInfo();
            if (tournamentInfo != null)
                TournamentUpdated?.Invoke(tournamentInfo.Value);
        }
    }
}
