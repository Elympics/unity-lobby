using System;

namespace ElympicsLobbyPackage.Session
{
    public class SessionManagerFatalError : Exception
    {
        public SessionManagerFatalError(string reason) : base($"Session manager fatal error. Reason: {reason}")
        {

        }
    }
}
