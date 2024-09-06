using System;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    internal class WebGLGameStatusCommunicator : IExternalGameStatusCommunicator
    {
        private readonly JsCommunicator _communicator;
        public WebGLGameStatusCommunicator(JsCommunicator communicator) => _communicator = communicator;

        public void GameFinished(int score) => _communicator.SendVoidMessage(VoidEventTypes.GameFinished, score);
        public void RttUpdated(TimeSpan rtt)
        {
            var debugMessage = JsCommunicationFactory.GetDebugMessageJson(DebugMessageTypes.RTT,
                new RTTDebugMessage()
                {
                    rtt = rtt.TotalMilliseconds
                });
            _communicator.SendVoidMessage(VoidEventTypes.Debug, debugMessage);
        }
        public void ApplicationInitialized() => _communicator.SendVoidMessage(VoidEventTypes.ApplicationInitialized, string.Empty);
    }
}
