using System;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    internal interface IJsCommunicatorRetriever
    {
        public event Action<string> ResponseObjectReceived;
        public event Action<WebMessageObject> WebObjectReceived;
    }
}
