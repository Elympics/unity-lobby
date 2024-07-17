using System;

namespace ElympicsLobbyPackage.Blockchain.Communication.Exceptions
{
    public class ChainIdMismatch : Exception
    {
        public ChainIdMismatch(string chainId, string expected) : base($"Expecting {expected} chainId instead of {chainId}")
        {

        }
    }
}
