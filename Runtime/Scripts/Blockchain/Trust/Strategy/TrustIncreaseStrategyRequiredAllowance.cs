using System;
using System.Numerics;
using Cysharp.Threading.Tasks;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    internal class TrustIncreaseStrategyRequiredAllowance : ITrustIncreaseStrategy
    {
        Func<UniTask<BigInteger>> CurrentAllowance { get; }
        BigInteger RequiredAllowance { get; }

        internal TrustIncreaseStrategyRequiredAllowance(Func<UniTask<BigInteger>> currentAllowance, BigInteger requiredAllowance)
        {
            CurrentAllowance = currentAllowance;
            RequiredAllowance = requiredAllowance;
        }

        public async UniTask<bool> IsStepRequired() => (await CurrentAllowance.Invoke()) < RequiredAllowance;
    }
}
