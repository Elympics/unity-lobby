using ElympicsLobbyPackage.Blockchain.Trust.Command;

namespace ElympicsLobbyPackage.Blockchain.Trust
{
    public static class TrustIncreaseCommandStateExtension
    {
        public static bool IsFinal(this TrustIncreaseCommandState state) => state is TrustIncreaseCommandState.Finished or TrustIncreaseCommandState.Failed;
        public static bool IsActive(this TrustIncreaseCommandState state) => state is TrustIncreaseCommandState.Signing or TrustIncreaseCommandState.Mining;
    }
}
