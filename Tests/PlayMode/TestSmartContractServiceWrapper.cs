using System.Collections;
using System.Collections.Generic;
using ElympicsLobbyPackage;
using SCS;
using UnityEngine;

namespace ElympicsLobby.Tests.PlayMode
{
    public class TestSmartContractServiceWrapper : MonoBehaviour, ISmartContractServiceWrapper
    {
        public ChainConfig? CurrentChain => new ChainConfig()
        {
            chainId = "11155111",
        };
    }
}
