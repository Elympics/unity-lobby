using Cysharp.Threading.Tasks;
using SCS;

namespace ElympicsLobbyPackage
{
    public interface IExternalContractOperations
    {
        public UniTask<string> GetValue<T>(SmartContract contract, string name, params string[] parameters);
        public UniTask<string> GetFunctionCallData(SmartContract contract, string functionName, params object[] parameters);
    }
}
