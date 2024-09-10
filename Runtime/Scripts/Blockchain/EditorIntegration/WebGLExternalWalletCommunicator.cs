#nullable enable
using System;
using System.Numerics;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.Communication.Exceptions;
using ElympicsLobbyPackage.Blockchain.Utils;
using ElympicsLobbyPackage.ExternalCommunication;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Protocol.WebMessages.Models;
using SCS;
using UnityEngine;
using TransactionToSign = ElympicsLobbyPackage.Blockchain.Communication.DTO.TransactionToSign;

namespace ElympicsLobbyPackage.Blockchain.EditorIntegration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class WebGLExternalWalletCommunicator : IExternalWalletCommunicator
    {
        private SmartContract.SmartContract TokenContract => SmartContract.SmartContract.ConvertFrom(_currentConfig!.Value.GetSmartContract(SmartContractType.ERC20Token));
        private SmartContract.SmartContract TrustContract => SmartContract.SmartContract.ConvertFrom(_currentConfig!.Value.GetSmartContract(SmartContractType.SecurityDeposit));

        private readonly JsCommunicator _communicator;
        private ChainConfig? _currentConfig;
        private IWalletConnectionListener _connectionListener;
        public WebGLExternalWalletCommunicator(JsCommunicator jsCommunicator, SmartContractServiceConfig scsConfig)
        {
            _communicator = jsCommunicator;
            var elympicsConfig = ElympicsConfig.Load();
            _currentConfig = scsConfig.GetChainConfigForGameId(elympicsConfig.GetCurrentGameConfig().GameId);
            _communicator.WebObjectReceived += OnWebObjectReceived;
        }

        private void OnWebObjectReceived(WebMessageObject messageObject)
        {
            switch (messageObject.type)
            {
                case WebMessages.WalletConnection:
                    OnWalletConnected(messageObject.message);
                    break;
            }
        }

        private void OnWalletConnected(string webMessageMessage)
        {
            var walletConnectedData = JsonUtility.FromJson<WalletConnectionMessage>(webMessageMessage);
            if (walletConnectedData.isConnected)
                _connectionListener.OnWalletConnected(walletConnectedData.address,walletConnectedData.chainId);
            else
                _connectionListener.OnWalletDisconnected();
        }

        public async UniTask<string> SignMessage<TInput>(string address, TInput message) => await _communicator.SendRequestMessage<TInput, string>(ReturnEventTypes.SignTypedData, message);

        public UniTask<bool> IsWalletAvailable() => UniTask.FromResult(true);

        public async UniTask<ConnectionResponse> Connect(BigInteger chainId)
        {
            var response = await _communicator.SendRequestMessage<string, ConnectionResponse>(ReturnEventTypes.Connect, chainId.ToString());
            if (response.chainId != _currentConfig!.Value.chainId)
                throw new ChainIdMismatch(response.chainId, _currentConfig!.Value.chainId);

            return response;
        }
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
        void IExternalWalletCommunicator.SetWalletConnectionListener(IWalletConnectionListener listener) => _connectionListener = listener;

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

        public void Dispose()
        {
            Debug.Log($"[{nameof(WebGLExternalWalletCommunicator)}] Dispose.");
            _communicator.WebObjectReceived -= OnWebObjectReceived;
        }
    }
}
