using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
namespace ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators
{
    internal interface IPlayPadEventListener
    {
        void OnWalletConnected(string address, string chainId);
        void OnWalletDisconnected();
        void OnTrustDepositFinished(TrustDepositTransactionFinishedWebMessage transaction);
    }
}
