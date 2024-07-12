using System;
namespace ElympicsLobbyPackage.ExternalCommunication
{
    internal interface IJsCommunicatorRetriever
    {
        public event Action<string> ResponseObjectReceived;
        public event Action<string> WebObjectReceived;
    }
}
