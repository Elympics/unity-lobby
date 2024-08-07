namespace ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators
{
    public interface IWalletConnectionListener
    {
        void OnWalletConnected(string address, string chainId);
        void OnWalletDisconnected();
    }
}
