namespace ElympicsLobbyPackage.Blockchain
{
    public interface ITokenAddressProvider
    {
        string GetAddress();
        int GetChainId();
    }
}
