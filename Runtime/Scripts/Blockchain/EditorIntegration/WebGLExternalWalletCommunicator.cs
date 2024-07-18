#nullable enable
using System;
using System.Numerics;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.Utils;
using ElympicsLobbyPackage.ExternalCommunication;
using SCS;
using UnityEngine;
using TransactionToSign = ElympicsLobbyPackage.Blockchain.Communication.DTO.TransactionToSign;

namespace ElympicsLobbyPackage.Blockchain.EditorIntegration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class WebGLExternalWalletCommunicator : IExternalWalletCommunicator
    {
        public event Action<string, string>? WalletConnected;
        public event Action? WalletDisconnected;

        private SmartContract.SmartContract TokenContract => SmartContract.SmartContract.ConvertFrom(_currentConfig!.Value.GetSmartContract(SmartContractType.ERC20Token));
        private SmartContract.SmartContract TrustContract => SmartContract.SmartContract.ConvertFrom(_currentConfig!.Value.GetSmartContract(SmartContractType.SecurityDeposit));

        private readonly JsCommunicator _communicator;
        private ChainConfig? _currentConfig;
        public WebGLExternalWalletCommunicator(JsCommunicator jsCommunicator, SmartContractServiceConfig scsConfig)
        {
            _communicator = jsCommunicator;
            var elympicsConfig = ElympicsConfig.Load();
            _currentConfig = scsConfig.GetChainConfigForGameId(elympicsConfig.GetCurrentGameConfig().GameId);
            _communicator.WebObjectReceived += OnWebObjectReceived;
        }

        private void OnWebObjectReceived(string messageObject)
        {
            Debug.Log($"[ApiConnector] Message received: {messageObject}");
            var webMessage = JsonUtility.FromJson<WebMessageObject>(messageObject);

            switch (webMessage.type)
            {
                case WebMessages.WalletConnection:
                    OnWalletConnected(webMessage.message);
                    break;
                default:
                    throw new ArgumentException($"Message type {webMessage.type} is not supported");
            }
        }
        private void OnWalletConnected(string webMessageMessage)
        {
            var walletConnectedData = JsonUtility.FromJson<WalletConnectionMessage>(webMessageMessage);
            if (walletConnectedData.isConnected)
                WalletConnected?.Invoke(walletConnectedData.address, walletConnectedData.chainId);
            else
                WalletDisconnected?.Invoke();
        }

        public async UniTask<string> SignMessage<TInput>(string address, TInput message) => await _communicator.SendRequestMessage<TInput, string>(ReturnEventTypes.SignTypedData, message);

        public UniTask<bool> IsWalletAvailable() => UniTask.FromResult(true);

        public async UniTask<ConnectionResponse> Connect(BigInteger chainId) => await _communicator.SendRequestMessage<string, ConnectionResponse>(ReturnEventTypes.Connect, chainId.ToString());
        public async UniTask<string> GetBalance(string owner)
        {
            var parameters = new[]
            {
                owner
            };
            return await GetValue(TokenContract, ValueCalls.BalanceOf, parameters);
        }
        public async UniTask<string> GetAllowance(string owner, string spender)
        {
            var parameters = new[]
            {
                owner,
                spender
            };
            return await GetValue(TokenContract, ValueCalls.Allowance, parameters);
        }
        public async UniTask<string> ApproveMax(string owner, string spender, BigInteger value)
        {
            var parameters = new[]
            {
                spender,
                value.ToString()
            };
            var data = await EncodeFunctionData(TokenContract, EncodeFunctionDataCalls.Approve, parameters);
            return await SendTransaction(owner, TokenContract.Address, owner, data);
        }

        public async UniTask<string> Deposit(string owner, string token, BigInteger value)
        {
            var parameters = new[]
            {
                token,
                value.ToString()
            };
            var data = await EncodeFunctionData(TrustContract, EncodeFunctionDataCalls.Deposit, parameters);
            return await SendTransaction(owner, TrustContract.Address, owner, data);
        }

        public void ExternalShowChainSelection() => _communicator.SendVoidMessage(VoidEventTypes.ShowChainSelectionUI, string.Empty);
        public void ExternalShowConnectToWallet() => _communicator.SendVoidMessage(VoidEventTypes.ShowConnectToWallet, string.Empty);
        public void ExternalShowAccountInfo() => _communicator.SendVoidMessage(VoidEventTypes.ShowAccountUI, string.Empty);

        internal async UniTask<InitializationResponse> SendInitializationMessage(InitializationMessage message) => await _communicator.SendRequestMessage<InitializationMessage, InitializationResponse>(ReturnEventTypes.Handshake, message);
        public async UniTask<string> SendTransaction(string address, string to, string from, string data)
        {
            var transaction = new TransactionToSign()
            {
                to = to,
                from = from,
                data = data,
            };
            return await _communicator.SendRequestMessage<TransactionToSign, string>(ReturnEventTypes.SendTransaction, transaction);
        }

        private async UniTask<string> EncodeFunctionData(SmartContract.SmartContract tokenInfo, string functionCall, params string[] parameters)
        {
            var message = new EncodeFunctionData()
            {
                contractAddress = tokenInfo.Address,
                ABI = tokenInfo.ABI,
                function = functionCall,
                parameters = parameters,
            };
            return await _communicator.SendRequestMessage<EncodeFunctionData, string>(ReturnEventTypes.EncodeFunctionData, message);
        }

        private async UniTask<string> GetValue(SmartContract.SmartContract tokenInfo, string valueName, params string[] parameters)
        {
            var message = new EncodeFunctionData()
            {
                contractAddress = tokenInfo.Address,
                ABI = tokenInfo.ABI,
                function = valueName,
                parameters = parameters,
            };
            return await _communicator.SendRequestMessage<EncodeFunctionData, string>(ReturnEventTypes.GetValue, message);
        }

        public void Dispose() => _communicator.WebObjectReceived -= OnWebObjectReceived;
    }
}
