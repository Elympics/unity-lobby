using ElympicsLobbyPackage.Blockchain.Trust.Command;
using ElympicsLobbyPackage.Blockchain.Wallet;
using SCS;

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    public abstract class TrustIncreaseSmartContractCommand : TrustIncreaseCommand
    {
        protected ChainConfig chainConfig;
        protected Web3Wallet Web3Wallet;

        protected TrustIncreaseSmartContractCommand(ChainConfig chainConfig, Web3Wallet web3Wallet)
        {
            this.chainConfig = chainConfig;
            this.Web3Wallet = web3Wallet;
        }

        protected void HandleEvent(TransactionEvent transactionEvent)
        {
            UnityEngine.Debug.Log($"[Command] handling event: {transactionEvent}");
            if (transactionEvent == TransactionEvent.TransactionHash && State == TrustIncreaseCommandState.Signing)
                ProceedToMiningStateIfPossible();
        }
    }
}
