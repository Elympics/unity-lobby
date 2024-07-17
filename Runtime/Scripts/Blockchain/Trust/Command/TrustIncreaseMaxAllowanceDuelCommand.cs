using System;
using ElympicsLobbyPackage.Blockchain.Trust.Command;
using ElympicsLobbyPackage.Blockchain.Wallet;
using SCS;

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    public class TrustIncreaseMaxAllowanceDuelCommand : TrustIncreaseMaxAllowanceCommand
    {
        public TrustIncreaseMaxAllowanceDuelCommand(ChainConfig chainConfig, Web3Wallet web3Wallet) : base(chainConfig, web3Wallet)
        {
        }

        public override SmartContractType SmartContractType => SmartContractType.OnChainDuel;
        protected override string Name => "Duel";

        public override string DescriptionForState(TrustIncreaseCommandState state)
        {
            return state switch
            {
                TrustIncreaseCommandState.NotStarted => "Needed for bet settlement",
                TrustIncreaseCommandState.Signing => "Needed for bet settlement",
                TrustIncreaseCommandState.Mining => "Needed for bet settlement",
                TrustIncreaseCommandState.Finished => "Ready to play",
                TrustIncreaseCommandState.Failed => "Try again",
                _ => throw new NotImplementedException()
            };
        }
    }
}
