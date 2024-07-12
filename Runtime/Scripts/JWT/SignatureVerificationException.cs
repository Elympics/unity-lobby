using System;

namespace ElympicsLobbyPackage.JWT
{
    public class SignatureVerificationException : Exception
    {
        public SignatureVerificationException(string message)
            : base(message)
        {
        }
    }
}
