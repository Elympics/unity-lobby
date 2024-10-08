using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Blockchain.Utils;
using Nethereum.ABI;
using Org.BouncyCastle.Math;
using SCS;
using UnityEngine;
using BigInteger = System.Numerics.BigInteger;

namespace ElympicsLobbyPackage
{
    public interface IExternalERC20SmartContractOperations
    {
        public UniTask<string> GetDecimals(SmartContract tokenContract);
        public UniTask<string> GetBalance(SmartContract tokenContract, string owner);
        public UniTask<string> GetSymbol(SmartContract tokenContract);
        public UniTask<string> GetName(SmartContract tokenContract);
        public UniTask<string> GetAllowance(SmartContract tokenContract, string owner, string spender);
        public UniTask<string> ApproveMax(SmartContract tokenContract, string owner, string spender) => Approve(tokenContract, owner, spender, IntType.MAX_UINT256_VALUE);
        public UniTask<string> Approve(SmartContract tokenContract, string owner, string spender, BigInteger value);
    }
}
