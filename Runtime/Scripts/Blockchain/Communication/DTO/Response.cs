using System;

namespace ElympicsLobbyPackage.Blockchain.EditorIntegration
{
    [Serializable]
    public class Response
    {
        public int ticket;
        public string type;
        public int status;
        public string response;

        public override string ToString() => $"Ticket: {ticket}, Status: {status} Response: {response}";
    }
}
