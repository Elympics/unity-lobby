using System.Collections;
using System.Collections.Generic;
using SCS;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    public interface ISmartContractServiceWrapper
    {
        public ChainConfig? CurrentChain { get; }
    }
}
