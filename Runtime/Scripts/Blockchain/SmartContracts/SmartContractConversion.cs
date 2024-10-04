
using SCS;
namespace ClashOfOrbs.Blockchain
{
    public static class SmartContractConversion
    {
        public static SmartContract ToOrbsContract(this SmartContractDTO contract)
        {
            return new()
            {
                Address = contract.Address,
                ABI = contract.ABI
            };
        }
    }
}
