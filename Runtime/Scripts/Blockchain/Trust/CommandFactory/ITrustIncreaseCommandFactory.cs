using System.Numerics;
using System.Collections.Generic;
using ElympicsLobbyPackage.Blockchain.Trust.Command;
using Cysharp.Threading.Tasks;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    public interface ITrustIncreaseCommandFactory
    {
        public UniTask<IEnumerable<TrustIncreaseCommand>> IncreaseTrustBy(BigInteger amount);
    }
}
