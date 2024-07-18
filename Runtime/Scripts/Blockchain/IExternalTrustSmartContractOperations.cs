using System.Numerics;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;

namespace ElympicsLobbyPackage
{
    public interface IExternalTrustSmartContractOperations
    {
        public void ShowTrustPanel();

        public UniTask<TrustState> GetTrustState();
    }
}
