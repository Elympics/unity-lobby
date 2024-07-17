using System;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain
{
    [Serializable]
    public struct TokenConfig
    {
        public Guid id;
        public int chainId;
        public int decimals;
        public string name;
        public string symbol;
        public string address;
    }
}
