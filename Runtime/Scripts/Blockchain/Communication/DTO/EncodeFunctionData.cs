using System;

namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
    [Serializable]
    public class EncodeFunctionData
    {
        public string contractAddress;
        public string ABI;
        public string function;
        public string[] parameters;
    }
}
