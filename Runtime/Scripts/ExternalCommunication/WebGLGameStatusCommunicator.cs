using ElympicsLobbyPackage.Blockchain.Communication;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    internal class WebGLGameStatusCommunicator : IExternalGameStatusCommunicator
    {
        private readonly JsCommunicator _communicator;
        public WebGLGameStatusCommunicator(JsCommunicator communicator) => _communicator = communicator;

        public void GameFinished(int score) => _communicator.SendVoidMessage(VoidEventTypes.GameFinished, score);
        public void ApplicationInitialized() => _communicator.SendVoidMessage(VoidEventTypes.ApplicationInitialized, string.Empty);
    }
}
