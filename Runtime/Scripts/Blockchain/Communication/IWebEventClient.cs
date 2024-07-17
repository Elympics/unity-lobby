namespace ElympicsLobbyPackage.Blockchain.Communication
{
    public interface IWebEventClient
    {
        void OnWalletConnected(string address);
        void OnWalletDisconnected();
    }
}
