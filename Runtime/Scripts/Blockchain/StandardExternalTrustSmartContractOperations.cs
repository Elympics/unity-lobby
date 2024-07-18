#nullable enable
using System;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
using UnityEngine;
namespace ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain
{
    public class StandardExternalTrustSmartContractOperations : IExternalTrustSmartContractOperations
    {
        private readonly ISmartContractServiceWrapper? _scs;
        public StandardExternalTrustSmartContractOperations(ISmartContractServiceWrapper? scs) => _scs = scs;
        public void ShowTrustPanel() => Debug.Log("Mock Deposit.");
        public async UniTask<TrustState> GetTrustState()
        {
            if (_scs == null)
                throw new NullReferenceException($"Make sure that {nameof(ElympicsExternalCommunicator)} gameObject has {nameof(ISmartContractServiceWrapper)} component attached.");

            return await _scs.GetTrustBalance();
        }
    }
}
