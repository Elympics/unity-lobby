using System.Numerics;
using Cysharp.Threading.Tasks;
using SCS;
using ElympicsLobbyPackage.Blockchain.Utils;

namespace ElympicsLobbyPackage.Blockchain
{
    public class Erc20SmartContractCommunicator: IExternalERC20SmartContractOperations
    {
        private readonly IExternalContractOperations _externalContractOperations;
        private readonly IExternalWalletOperations _externalWalletOperator;
        public Erc20SmartContractCommunicator(IExternalContractOperations externalContractOperations, IExternalWalletOperations externalWalletOperator)
        {
            _externalContractOperations = externalContractOperations;
            _externalWalletOperator = externalWalletOperator;
        }
        public async UniTask<string> GetDecimals(SmartContract tokenContract)
        {
            if (tokenContract.Type is not SmartContractType.ERC20Token)
                throw new ERC20SmartContractException("Token contract is not of ERC20Token type.");
            return await _externalContractOperations.GetValue<int>(tokenContract, ValueCalls.Decimals);
        }

        public async UniTask<string> GetBalance(SmartContract tokenContract, string owner)
        {
            if (tokenContract.Type is not SmartContractType.ERC20Token)
                throw new ERC20SmartContractException("Token contract is not of ERC20Token type.");
            return await _externalContractOperations.GetValue<BigInteger>(tokenContract, ValueCalls.BalanceOf, owner);
        }

        public async UniTask<string> GetSymbol(SmartContract tokenContract)
        {
            if (tokenContract.Type is not SmartContractType.ERC20Token)
                throw new ERC20SmartContractException("Token contract is not of ERC20Token type.");
            return await _externalContractOperations.GetValue<string>(tokenContract, ValueCalls.Symbol);
        }

        public async UniTask<string> GetName(SmartContract tokenContract)
        {
            if (tokenContract.Type is not SmartContractType.ERC20Token)
                throw new ERC20SmartContractException("Token contract is not of ERC20Token type.");
            return await _externalContractOperations.GetValue<string>(tokenContract, ValueCalls.Name);
        }

        public async UniTask<string> GetAllowance(SmartContract tokenContract, string owner, string spender)
        {
            if (tokenContract.Type is not SmartContractType.ERC20Token)
                throw new ERC20SmartContractException("Token contract is not of ERC20Token type.");
            return await _externalContractOperations.GetValue<BigInteger>(tokenContract, ValueCalls.Allowance, owner, spender);
        }

        async UniTask<string> IExternalERC20SmartContractOperations.Approve(SmartContract tokenContract, string owner, string spender, BigInteger value)
        {
            if (tokenContract.Type is not SmartContractType.ERC20Token)
                throw new ERC20SmartContractException("Token contract is not of ERC20Token type.");
            var parameters = new[]
            {
                spender,
                value.ToString()
            };

            var data = await _externalContractOperations.GetFunctionCallData(tokenContract, EncodeFunctionDataCallsERC20.Approve, parameters);
            return await _externalWalletOperator.SendTransaction(tokenContract.Address, owner, data);
        }
    }
}
