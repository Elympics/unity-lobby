using System;
using System.Numerics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage.Blockchain.EditorIntegration;
using ElympicsLobbyPackage.ExternalCommunication;
using JetBrains.Annotations;
using Nethereum.ABI;
using SCS;
using UnityEngine;

#nullable enable

namespace ElympicsLobbyPackage.Blockchain.Wallet
{
    [RequireComponent(typeof(ISmartContractServiceWrapper))]
    [DefaultExecutionOrder(ElympicsLobbyExecutionOrders.Web3Wallet)]
    public class Web3Wallet : ElympicsEthSigner, IWallet
    {
        [PublicAPI]
        public event Action<WalletConnectionStatus>? WalletConnectionUpdated;
        internal event Action<WalletConnectionStatus>? WalletConnectionUpdatedInternal;
        [SerializeField] private SmartContractServiceConfig config;
        [SerializeField] private StandaloneBrowserJsConfig _browserJsConfig;
        public override string Address => _address;

        private string? _address;
        private ISmartContractServiceWrapper _scs;

        private static IExternalWalletCommunicator WalletCommunicator => ElympicsExternalCommunicator.Instance!.WalletCommunicator!;
        private void Awake()
        {
            _scs = GetComponent<ISmartContractServiceWrapper>();
        }

        private void Start() => Subscribe();

        public async UniTask<string> ConnectWeb3()
        {
            var result = await WalletCommunicator.Connect(ChainId);
            _address = result.address;
            return _address;
        }

        public async UniTask<string> SignTypedDataV4(string message)
        {
            Debug.Log($"[Wallet] Sign message {message} using address {Address}");
            return await WalletCommunicator.SignMessage(Address, message);
        }

        public async UniTask<string> SendTransaction(SendTransactionWalletRequest value)
        {
            try
            {
                return await WalletCommunicator.SendTransaction(Address, value.To, value.From, value.Data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw e;
            }
        }

        public override BigInteger ChainId => BigInteger.Parse(_scs.CurrentChain!.Value.chainId);

        public override async UniTask<string> SignAsync(string message, CancellationToken ct = default)
        {
            Debug.Log($"[Wallet] Data to sign: {message} using address {Address}");
            return await WalletCommunicator!.SignMessage(Address, message);
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
            return BigInteger.Parse(await WalletCommunicator.GetBalance(Address));
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
            await WalletCommunicator.ApproveMax(Address, spender, IntType.MAX_UINT256_VALUE);
        }

        public async UniTask Deposit(BigInteger amount)
        {
            Debug.Log($"[Wallet] Depositing to Trust... [{amount}]");
            await WaitForConfig();
            var tokenAddress = _scs.CurrentChain!.Value.GetSmartContract(SmartContractType.ERC20Token).Address;
            await WalletCommunicator.Deposit(Address, tokenAddress, amount);
        }

        public async UniTask<BigInteger> GetAllowance(string spender)
        {
            Debug.Log("[Wallet] Getting token allowance...");
            await WaitForConfig();
            return BigInteger.Parse(await WalletCommunicator.GetAllowance(Address, spender));
        }
        public void ExternalShowChainSelection()
        {
            WalletCommunicator.ExternalShowChainSelection();
        }
        public void ExternalShowConnectToWallet()
        {
            WalletCommunicator.ExternalShowConnectToWallet();
        }
        public void ExternalShowAccountInfo()
        {
            WalletCommunicator.ExternalShowAccountInfo();
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
            WalletConnectionUpdatedInternal?.Invoke(WalletConnectionStatus.Connected);
            WalletConnectionUpdated?.Invoke(WalletConnectionStatus.Connected);

        }
        private void OnWalletDisconnected()
        {
            _address = null;
            WalletConnectionUpdatedInternal?.Invoke(WalletConnectionStatus.Disconnected);
            WalletConnectionUpdated?.Invoke(WalletConnectionStatus.Disconnected);
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {

            ElympicsExternalCommunicator.Instance.WalletConnected += OnWalletConnected;
            ElympicsExternalCommunicator.Instance.WalletDisconnected += OnWalletDisconnected;
        }
        private void UnSubscribe()
        {

            ElympicsExternalCommunicator.Instance.WalletConnected -= OnWalletConnected;
            ElympicsExternalCommunicator.Instance.WalletDisconnected -= OnWalletDisconnected;
        }
    }
}
