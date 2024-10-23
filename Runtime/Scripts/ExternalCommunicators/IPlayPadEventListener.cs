using Elympics.Models.Authentication;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
namespace ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators
{
    internal interface IPlayPadEventListener
    {
        void OnWalletConnected(string address, string chainId);
        void OnWalletDisconnected();
        void OnAuthChanged(AuthData authData);
        void OnTrustDepositFinished(TrustDepositTransactionFinishedWebMessage transaction);
    }
}
