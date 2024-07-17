namespace ElympicsLobbyPackage.Blockchain.SmartContract
{
    public struct SmartContract
    {
        public string Address;
        public string ABI;

        public static SmartContract ConvertFrom(SCS.SmartContract smartContract)
        {
            return new SmartContract()
            {
                Address = smartContract.Address,
                ABI = smartContract.ABI
            };
        }
    }
}
