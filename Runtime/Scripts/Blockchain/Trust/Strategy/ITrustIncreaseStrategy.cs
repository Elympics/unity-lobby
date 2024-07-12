using Cysharp.Threading.Tasks;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    internal interface ITrustIncreaseStrategy
    {
        UniTask<bool> IsStepRequired();
    }
}
