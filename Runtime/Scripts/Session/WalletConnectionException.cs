using System;

namespace ElympicsLobbyPackage.Session
{
    public class WalletConnectionException : Exception
    {
        public WalletConnectionException(string message) : base(message)
        { }
    }
}
