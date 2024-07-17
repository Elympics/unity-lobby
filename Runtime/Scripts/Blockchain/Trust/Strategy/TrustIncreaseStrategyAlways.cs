using Cysharp.Threading.Tasks;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    internal class TrustIncreaseStrategyAlways : ITrustIncreaseStrategy
    {
        public UniTask<bool> IsStepRequired() => UniTask.FromResult<bool>(true);
    }
}
