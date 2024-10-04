using System;
using System.Numerics;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;

namespace ElympicsLobbyPackage
{
    internal class WebGlTrustSmartContractCommunicator : IExternalTrustSmartContractOperations
    {
        private readonly JsCommunicator _jsCommunicator;
        private readonly IElympicsLobbyWrapper _lobbyWrapper;
        public WebGlTrustSmartContractCommunicator(JsCommunicator jsCommunicator, IElympicsLobbyWrapper lobbyWrapper)
        {
            _jsCommunicator = jsCommunicator;
            _lobbyWrapper = lobbyWrapper;
        }

        public void ShowTrustPanel() => _jsCommunicator.SendVoidMessage(VoidEventTypes.IncreaseTrust,
            new IncreaseTrust
            {
                jwtToken = _lobbyWrapper.AuthData.JwtToken
            });
        public async UniTask<TrustState> GetTrustState()
        {
            var result = await _jsCommunicator.SendRequestMessage<CheckDepositMessage, CheckDepositResponse>(ReturnEventTypes.GetTrustState,
                new CheckDepositMessage()
                {
                    jwtToken = _lobbyWrapper.AuthData.JwtToken
                });

            return new TrustState()
            {
                AvailableAmount = result.Available,
                TotalAmount = result.totalAmount
            };
        }
    }
}
