using System;
using Cysharp.Threading.Tasks;
using Elympics;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
using Elympics;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
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
        public void RegisterWallet(IWallet wallet) => _scs.RegisterWallet(wallet);
        public async UniTask<TrustState> GetTrustBalance()
        {
            var currentConfig = ElympicsConfig.Load();
            var gameId = currentConfig!.GetCurrentGameConfig()!.GameId;
            var result = await _scs.GetDepositState(gameId!);
            foreach (var tokenDeposit in result)
            {
                var trustToken = _scs.CurrentChain.Value!.GetSmartContract(SmartContractType.ERC20Token);
                if (tokenDeposit.TokenAddress == trustToken.Address)
                {
                    return new TrustState()
                    {
                        AvailableAmount = (float)WeiConverter.FromWei(tokenDeposit.AvailableAmount, 6),
                        TotalAmount = (float)WeiConverter.FromWei(tokenDeposit.ActualAmount, 6)
                    };
                }
            }
            throw new Exception("Couldn't find token.");
        }
    }
}
