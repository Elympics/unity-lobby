#nullable enable
using System;
using System.Numerics;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators;
using ElympicsLobbyPackage.Blockchain;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    public interface IExternalWalletCommunicator : IExternalWalletOperations, IDisposable
    {
        public UniTask<ConnectionResponse> Connect();
        public void ExternalShowChainSelection();
        public void ExternalShowConnectToWallet();
        public void ExternalShowAccountInfo();
        internal void SetPlayPadEventListener(IPlayPadEventListener listener);
    }
}
