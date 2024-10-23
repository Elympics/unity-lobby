using System;

namespace ElympicsLobbyPackage
{
    public class ProtocolException : Exception
    {
        public ProtocolException(string message, string messageType) : base($"[{messageType}] {message}")
        {

        }
    }
}
