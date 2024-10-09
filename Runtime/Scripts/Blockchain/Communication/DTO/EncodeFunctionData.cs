using System;
using SCS;

namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
    [Serializable]
    public class EncodeFunctionData
    {
        public string chainId;
        public string contractAddress;
        public string ABI;
        public string function;
        public string[] parameters;

        public static EncodeFunctionData Create(SmartContract tokenInfo, string functionCall, params string[] parameters) => new()
        {
            chainId = tokenInfo.ChainId ?? string.Empty,
            contractAddress = tokenInfo.Address,
            ABI = tokenInfo.ABI,
            function = functionCall,
            parameters = parameters,
        };
    }
}
