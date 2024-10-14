#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    internal static class SmartContractExt
    {
        public static SmartContract ToSmartContract(this SCS.SmartContract elympicsSmartContract, string? chainId) => new()
        {
            ChainId = chainId,
            Address = elympicsSmartContract.Address,
            ABI = elympicsSmartContract.ABI
        };
    }
}
