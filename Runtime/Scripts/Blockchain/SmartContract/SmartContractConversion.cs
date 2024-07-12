using ElympicsLobbyPackage.Blockchain.SmartContract;
namespace ClashOfOrbs.Blockchain
{
    public static class SmartContractConversion
    {
        public static SmartContract ToOrbsContract(this SCS.SmartContract contract)
        {
            return new()
            {
                Address = contract.Address,
                ABI = contract.ABI
            };
        }
    }
}
