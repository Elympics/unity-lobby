using System;

namespace ElympicsLobbyPackage.Blockchain.Communication.Exceptions
{
    public class ResponseException : Exception
    {
        public readonly int Code;
        public ResponseException(int statusCode, string errorMessage) : base(errorMessage)
        {
            Code = statusCode;
        }
    }
}
