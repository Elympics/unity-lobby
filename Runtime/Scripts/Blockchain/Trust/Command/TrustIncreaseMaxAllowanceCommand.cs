using System;
using ElympicsLobbyPackage.Blockchain.Trust.Command;
using ElympicsLobbyPackage.Blockchain.Wallet;
using Cysharp.Threading.Tasks;
using SCS;

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    public abstract class TrustIncreaseMaxAllowanceCommand : TrustIncreaseSmartContractCommand
    {
        protected TrustIncreaseMaxAllowanceCommand(ChainConfig chainConfig, Web3Wallet web3Wallet) : base(chainConfig, web3Wallet)
        {
        }

        public abstract SmartContractType SmartContractType { get; }
        protected abstract string Name { get; }

        protected override async UniTask DoExecute()
        {
            var contract = chainConfig.GetSmartContract(SmartContractType).Address;
            await Web3Wallet.ApproveMax(contract);
        }

        public override int EstimatedTimeMilliseconds => 15 * 1000;
        public override string TitleForState(TrustIncreaseCommandState state)
        {
            return state switch
            {
                TrustIncreaseCommandState.NotStarted => $"Approve {Name}",
                TrustIncreaseCommandState.Signing => $"Approve {Name}",
                TrustIncreaseCommandState.Mining => $"Approving {Name}",
                TrustIncreaseCommandState.Finished => "Approved",
                TrustIncreaseCommandState.Failed => "Failed",
                _ => throw new NotImplementedException()
            };
        }

    }
}
