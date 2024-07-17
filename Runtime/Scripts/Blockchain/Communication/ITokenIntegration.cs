#nullable enable
using System.Numerics;
using Cysharp.Threading.Tasks;

namespace ElympicsLobbyPackage.Blockchain.Communication
{
    public interface ITokenIntegration
    {
        public UniTask<int> GetDecimals();
        public UniTask<BigInteger> GetBalance();
        public UniTask<string> GetName();
        public UniTask<string> GetSymbol();
        public UniTask ApproveMax(string spender);
        public UniTask<BigInteger> GetAllowance(string spender);
    }
}
