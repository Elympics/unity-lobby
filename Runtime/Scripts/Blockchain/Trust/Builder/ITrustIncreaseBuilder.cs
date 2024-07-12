using SCS;
using System.Numerics;
using System.Collections.Generic;
using ElympicsLobbyPackage.Blockchain.Trust.Command;
using ElympicsLobbyPackage.Blockchain.Wallet;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    internal interface ITrustIncreaseBuilder
    {
        void AddIncreaseDuelAllowance(ChainConfig chainConfig, Web3Wallet wallet);
        void AddIncreaseTrustAllowance(ChainConfig chainConfig, Web3Wallet wallet);
        void AddDepositTrust(ChainConfig chainConfig, Web3Wallet wallet, BigInteger amount);
        IEnumerable<TrustIncreaseCommand> Build();
    }
}
