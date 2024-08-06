using System;
using System.Numerics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage.Blockchain.EditorIntegration;
using ElympicsLobbyPackage.ExternalCommunication;
using Nethereum.ABI;
using SCS;
using UnityEngine;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain.Wallet
{
    [RequireComponent(typeof(SmartContractService))]
    [DefaultExecutionOrder(ExecutionOrders.Web3Wallet)]
    public class Web3Wallet : ElympicsEthSigner, IWallet
    {
        public event Action<WalletConnectionStatus>? WalletConnectionUpdated;
        [SerializeField] private SmartContractServiceConfig config;
        [SerializeField] private StandaloneBrowserJsConfig _browserJsConfig;
        public override string Address => _address;

        private string? _address;
        private SmartContractService _scs;

        private IExternalWalletCommunicator _walletCommunicator;

        private void Awake()
        {
            _scs = GetComponent<SmartContractService>();
            var communicator = FindObjectOfType<ElympicsExternalCommunicator>();
#if UNITY_EDITOR || !UNITY_WEBGL
            if (communicator.WalletCommunicator is null)
                communicator.SetCustomExternalWalletCommunicator(new StandaloneExternalWalletCommunicator(_browserJsConfig, _scs));
#else
            communicator.WalletCommunicator.WalletConnected += OnWalletConnected;
            communicator.WalletCommunicator.WalletDisconnected += OnWalletDisconnected;
#endif
            _walletCommunicator = communicator.WalletCommunicator!;
        }

        public async UniTask<string> ConnectWeb3()
        {
            var result = await _walletCommunicator.Connect(ChainId);
            _address = result.address;
            return _address;
        }

        public async UniTask<string> SignTypedDataV4(string message)
        {
            Debug.Log($"[Wallet] Sign message {message} using address {Address}");
            return await _walletCommunicator.SignMessage(Address, message);
        }

        public async UniTask<string> SendTransaction(SendTransactionWalletRequest value)
        {
            try
            {
                return await _walletCommunicator.SendTransaction(Address, value.To, value.From, value.Data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw e;
            }
        }

        public override BigInteger ChainId =>
            BigInteger.Parse(config.GetChainConfigForGameId(ElympicsConfig.Load().GetCurrentGameConfig().GameId).Value.chainId);

        public override async UniTask<string> SignAsync(string message, CancellationToken ct = default)
        {
            Debug.Log($"[Wallet] Data to sign: {message} using address {Address}");
            return await _walletCommunicator!.SignMessage(Address, message);
        }

        public async UniTask<int> GetDecimals()
        {
            Debug.Log("[Wallet] Getting decimals for token...");
            await WaitForConfig();
            return _scs.CurrentChain!.Value.decimals;
        }

        public async UniTask<BigInteger> GetBalance()
        {
            await WaitForConfig();
            return BigInteger.Parse(await _walletCommunicator.GetBalance(Address));
        }

        //TODO: implement this later
        public async UniTask<string> GetName()
        {
            await WaitForConfig();
            return await UniTask.FromResult("TestName");
            //return await _browserJs.GetName();
        }


        //TODO: implement this later
        public async UniTask<string> GetSymbol()
        {
            await WaitForConfig();
            return await UniTask.FromResult("TestSymbol");
            //return await _browserJs.GetSymbol();
        }

        public async UniTask ApproveMax(string spender)
        {
            Debug.Log($"[Wallet] Approving token spend for {spender}...");
            await WaitForConfig();
            await _walletCommunicator.ApproveMax(Address, spender, IntType.MAX_UINT256_VALUE);
        }

        public async UniTask Deposit(BigInteger amount)
        {
            Debug.Log($"[Wallet] Depositing to Trust... [{amount}]");
            await WaitForConfig();
            var tokenAddress = _scs.CurrentChain!.Value.GetSmartContract(SmartContractType.ERC20Token).Address;
            await _walletCommunicator.Deposit(Address, tokenAddress, amount);
        }

        public async UniTask<BigInteger> GetAllowance(string spender)
        {
            Debug.Log("[Wallet] Getting token allowance...");
            await WaitForConfig();
            return BigInteger.Parse(await _walletCommunicator.GetAllowance(Address, spender));
        }
        public void ExternalShowChainSelection()
        {
            _walletCommunicator.ExternalShowChainSelection();
        }
        public void ExternalShowConnectToWallet()
        {
            _walletCommunicator.ExternalShowConnectToWallet();
        }
        public void ExternalShowAccountInfo()
        {
            _walletCommunicator.ExternalShowAccountInfo();
        }


        private async UniTask WaitForConfig()
        {
            await UniTask.WaitUntil(() => _scs.CurrentChain != null);
        }

        private void OnWalletConnected(string address, string chainId)
        {
            if (_address == address)
                return;

            _address = address;
            WalletConnectionUpdated?.Invoke(WalletConnectionStatus.Connected);

        }
        private void OnWalletDisconnected()
        {
            _address = null;
            WalletConnectionUpdated?.Invoke(WalletConnectionStatus.Disconnected);
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            _walletCommunicator.WalletConnected -= OnWalletConnected;
            _walletCommunicator.WalletDisconnected -= OnWalletDisconnected;
#endif
        }
    }
}
