using System;

namespace ElympicsLobbyPackage.Blockchain
{
    public class ERC20SmartContractException : Exception
    {
        public ERC20SmartContractException(string message) : base(message)
        { }
    }

}
