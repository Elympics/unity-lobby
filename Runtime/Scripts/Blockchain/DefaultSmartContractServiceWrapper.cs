using SCS;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    [RequireComponent(typeof(SmartContractService))]
    [DefaultExecutionOrder(ElympicsLobbyExecutionOrders.DefaultSmartContractServiceWrapper)]
    public class DefaultSmartContractServiceWrapper : MonoBehaviour, ISmartContractServiceWrapper
    {
        private SmartContractService _scs;
        private void Awake()
        {
            _scs = GetComponent<SmartContractService>();
        }
        public ChainConfig? CurrentChain => _scs.CurrentChain;
    }
}
