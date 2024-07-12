namespace ElympicsLobbyPackage.Blockchain.Trust.Command
{
    public enum TrustIncreaseCommandState
    {
        NotStarted,
        Signing,
        Mining,
        Finished,
        Failed
    }
}
