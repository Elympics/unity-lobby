using System;
using ElympicsLobbyPackage.Blockchain.Trust.Command;
using ElympicsLobbyPackage.Blockchain.Wallet;
using SCS;

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    public class TrustIncreaseMaxAllowanceTrustCommand : TrustIncreaseMaxAllowanceCommand
    {
        public TrustIncreaseMaxAllowanceTrustCommand(ChainConfig chainConfig, Web3Wallet web3Wallet) : base(chainConfig, web3Wallet)
        {
        }

        public override SmartContractType SmartContractType => SmartContractType.SecurityDeposit;
        protected override string Name => "Trust";

        public override string DescriptionForState(TrustIncreaseCommandState state)
        {
            return state switch
            {
                TrustIncreaseCommandState.NotStarted => "Needed to increase Trust",
                TrustIncreaseCommandState.Signing => "Needed to increase Trust",
                TrustIncreaseCommandState.Mining => "Needed to increase Trust",
                TrustIncreaseCommandState.Finished => "Ready to increase Trust",
                TrustIncreaseCommandState.Failed => "Try again",
                _ => throw new NotImplementedException()
            };
        }
    }
}
