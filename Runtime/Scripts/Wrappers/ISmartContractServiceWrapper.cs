using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
using SCS;

namespace ElympicsLobbyPackage
{
    public interface ISmartContractServiceWrapper
    {
        public ChainConfig? CurrentChain { get; }
        public void RegisterWallet(IWallet wallet);
        UniTask<TrustState> GetTrustBalance();
    }
}
