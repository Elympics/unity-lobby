
using Cysharp.Threading.Tasks;
namespace ElympicsLobbyPackage.Blockchain
{
    public interface IExternalWalletOperations
    {
        public UniTask<string> SignMessage<TInput>(string address, TInput message);
        public UniTask<string> SendTransaction(string to, string from, string data);
    }
}
