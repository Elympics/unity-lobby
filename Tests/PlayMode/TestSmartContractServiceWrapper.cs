using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
using SCS;
using UnityEngine;

namespace ElympicsLobby.Tests.PlayMode
{
    public class TestSmartContractServiceWrapper : MonoBehaviour, ISmartContractServiceWrapper
    {
        public ChainConfig? CurrentChain => new ChainConfig()
        {
            ChainId = "11155111",
        };

        public void RegisterWallet(IWallet wallet) => Debug.Log("Wallet registered.");

        public async UniTask<TrustState> GetTrustBalance() => new();
    }
}
