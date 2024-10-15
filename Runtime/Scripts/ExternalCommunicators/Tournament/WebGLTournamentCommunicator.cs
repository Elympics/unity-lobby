#nullable enable
using System;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.Tournament;
using ElympicsLobbyPackage.Tournament.Util;
using UnityEngine;

namespace ElympicsLobbyPackage.ExternalCommunication.Tournament
{
    internal class WebGLTournamentCommunicator : IExternalTournamentCommunicator, IWebMessageReceiver
    {
        public event Action<TournamentInfo>? TournamentUpdated;
        public async UniTask<CanPlayTournamentResponse> CanPlayTournament() => await _jsCommunicator.SendRequestMessage<string, CanPlayTournamentResponse>(ReturnEventTypes.CanPlayTournament, string.Empty);

        private readonly JsCommunicator _jsCommunicator;
        public WebGLTournamentCommunicator(JsCommunicator jsCommunicator)
        {
            _jsCommunicator = jsCommunicator;
            _jsCommunicator.RegisterIWebEventReceiver(this, Blockchain.Communication.WebMessages.TournamentUpdated);
        }
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
