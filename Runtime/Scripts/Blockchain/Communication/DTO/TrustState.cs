namespace ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO
{
    public struct TrustState
    {
        public float TotalAmount;
        public float AvailableAmount;

        public static TrustState Noop => new TrustState()
        {
            TotalAmount = 0.0f,
            AvailableAmount = 0.0f
        };
    }
}
