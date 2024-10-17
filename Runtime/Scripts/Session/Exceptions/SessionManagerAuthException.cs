using System;

namespace ElympicsLobbyPackage
{
    public class SessionManagerAuthException : Exception
    {
        public SessionManagerAuthException(string message) : base(message)
        {
        }
    }
}
