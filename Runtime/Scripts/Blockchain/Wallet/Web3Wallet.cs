using System;
using System.Numerics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage.Blockchain.Communication.Exceptions;
using ElympicsLobbyPackage.Blockchain.EditorIntegration;
using JetBrains.Annotations;
using Nethereum.ABI;
using SCS;
using UnityEngine;

namespace ElympicsLobbyPackage.Blockchain.Wallet
{
	public class Web3Wallet : ElympicsEthSigner, IWallet
	{
		[CanBeNull] public event Action<WalletConnectionStatus> WalletConnectionUpdated;
		private SmartContractService _scs => _lazyScs.Value;

		private readonly Lazy<SmartContractService> _lazyScs = new(FindObjectOfType<SmartContractService>);
		[SerializeField] private SmartContractServiceConfig config;
		[SerializeField] private StandaloneBrowserJsConfig _browserJsConfig;
		public override string Address => _address;

		private string _address;

        private ElympicsExternalCommunicator _communicator;

		private void Awake()
        {
            _communicator = FindObjectOfType<ElympicsExternalCommunicator>();
#if UNITY_EDITOR || !UNITY_WEBGL
            _communicator.SetCustomExternalWalletCommunicator(new StandaloneExternalWalletCommunicator(_browserJsConfig,_scs));
#else
            _communicator.WalletCommunicator.WalletConnected+= OnWalletConnected;
            _communicator.WalletCommunicator.WalletDisconnected += OnWalletDisconnected;
#endif
		}

		public async UniTask<string> ConnectWeb3()
		{
			var result = await _communicator.WalletCommunicator.Connect(ChainId);
			if (result.chainId != ChainId.ToString())
			{
				throw new ChainIdMismatch(result.chainId, ChainId.ToString());
			}
			_address = result.address;
			return _address;
		}

		public async UniTask<string> SignTypedDataV4(string message)
		{
			Debug.Log($"[Wallet] Sign message {message} using address {Address}");
			return await _communicator.WalletCommunicator.SignMessage(Address, message);
		}

		public async UniTask<string> SendTransaction(SendTransactionWalletRequest value)
		{
			try
			{
				return await _communicator.WalletCommunicator.SendTransaction(Address, value.To, value.From, value.Data);
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
			return await _communicator.WalletCommunicator.SignMessage(Address, message);
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
			return BigInteger.Parse(await _communicator.WalletCommunicator.GetBalance(Address));
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
			await _communicator.WalletCommunicator.ApproveMax(Address, spender, IntType.MAX_UINT256_VALUE);
		}

		public async UniTask Deposit(BigInteger amount)
		{
			Debug.Log($"[Wallet] Depositing to Trust... [{amount}]");
			await WaitForConfig();
			var tokenAddress = _scs.CurrentChain!.Value.GetSmartContract(SmartContractType.ERC20Token).Address;
			await _communicator.WalletCommunicator.Deposit(Address, tokenAddress, amount);
		}

		public async UniTask<BigInteger> GetAllowance(string spender)
		{
			Debug.Log("[Wallet] Getting token allowance...");
			await WaitForConfig();
			return BigInteger.Parse(await _communicator.WalletCommunicator.GetAllowance(Address, spender));
		}
        public void ExternalShowChainSelection()
        {
            _communicator.WalletCommunicator.ExternalShowChainSelection();
        }
        public void ExternalShowConnectToWallet()
        {
            _communicator.WalletCommunicator.ExternalShowConnectToWallet();
        }
        public void ExternalShowAccountInfo()
        {
            _communicator.WalletCommunicator.ExternalShowAccountInfo();
        }


		private async UniTask WaitForConfig()
		{
			await UniTask.WaitUntil(() => _scs.CurrentChain != null);
		}

		private void OnDestroy() => _communicator!.WalletCommunicator.Dispose();
        private void OnWalletConnected(string address, string chainId)
        {
            if ((_address ?? string.Empty) == (address ?? string.Empty))
                return;

			var status = string.IsNullOrEmpty(_address) ? WalletConnectionStatus.Connected : WalletConnectionStatus.Reconnected;
			_address = address;
			WalletConnectionUpdated?.Invoke(status);

		}
		private void OnWalletDisconnected()
		{
			_address = null;
			WalletConnectionUpdated?.Invoke(WalletConnectionStatus.Disconnected);
		}
	}
}
