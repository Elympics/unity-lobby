using System;
using System.Numerics;
using ElympicsLobbyPackage.Blockchain.Trust.Command;
using ElympicsLobbyPackage.Blockchain.Wallet;
using Cysharp.Threading.Tasks;
using SCS;

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    public class TrustIncreaseDepositCommand : TrustIncreaseSmartContractCommand
    {
        public TrustIncreaseDepositCommand(ChainConfig chainConfig, Web3Wallet web3Wallet, BigInteger amount) : base(chainConfig, web3Wallet)
        {
            Amount = amount;
        }

        public override int EstimatedTimeMilliseconds => 15 * 1000;

        public BigInteger Amount { get; }

        public override string DescriptionForState(TrustIncreaseCommandState state)
        {
            return state switch
            {
                TrustIncreaseCommandState.NotStarted => "",
                TrustIncreaseCommandState.Signing => "",
                TrustIncreaseCommandState.Mining => "",
                TrustIncreaseCommandState.Finished => "",
                TrustIncreaseCommandState.Failed => "",
                _ => throw new NotImplementedException()
            };
        }

        public override string TitleForState(TrustIncreaseCommandState state)
        {
            return state switch
            {
                TrustIncreaseCommandState.NotStarted => $"Deposit Trust",
                TrustIncreaseCommandState.Signing => $"Confirm Trust",
                TrustIncreaseCommandState.Mining => $"Depositing Trust",
                TrustIncreaseCommandState.Finished => "Deposited",
                TrustIncreaseCommandState.Failed => "Failed",
                _ => throw new NotImplementedException()
            };
        }

        protected async override UniTask DoExecute()
        {
            await Web3Wallet.Deposit(Amount);
        }
    }
}
