#nullable enable
using System.Numerics;
using Cysharp.Threading.Tasks;

namespace ElympicsLobbyPackage.Blockchain.Communication
{
    public interface ITrustIntegration
    {
        public UniTask Deposit(BigInteger amount);
    }
}
