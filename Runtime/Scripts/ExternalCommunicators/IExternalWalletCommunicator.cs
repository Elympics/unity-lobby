#nullable enable
using System;
using System.Numerics;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using Cysharp.Threading.Tasks;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    public interface IExternalWalletCommunicator: IDisposable
    {
        public event Action<string, string> WalletConnected;
        public event Action WalletDisconnected;
        public UniTask<string> SignMessage<TInput>(string address, TInput message);
        public UniTask<ConnectionResponse> Connect(BigInteger chainId);
        public UniTask<string> GetBalance(string owner);
        public UniTask<string> GetAllowance(string owner, string spender);
        public UniTask<string> ApproveMax(string owner, string spender, BigInteger value);
        public UniTask<string> SendTransaction(string address, string to, string from, string data);
        public UniTask<string> Deposit(string owner, string token, BigInteger value);
        public void ExternalShowChainSelection();
        public void ExternalShowConnectToWallet();
        public void ExternalShowAccountInfo();
    }
}
