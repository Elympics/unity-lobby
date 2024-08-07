#nullable enable
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Nethereum.ABI.EIP712;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using UnityEngine;
using System.Linq;
using System;
using System.Numerics;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.Blockchain.Utils;
using ElympicsLobbyPackage.ExternalCommunication;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators;
using Nethereum.Hex.HexConvertors.Extensions;
using SCS;

namespace ElympicsLobbyPackage.Blockchain.EditorIntegration
{
	public class StandaloneExternalWalletCommunicator : IExternalWalletCommunicator
	{
		private EthECKey _key;
		private IWeb3 _web3;
		private Account _account;
		private Eip712TypedDataSigner _signer;
		private Dictionary<string, Contract> _contracts;
		private readonly StandaloneBrowserJsConfig _config;
		private readonly SmartContractService _scs;
		private SmartContract.SmartContract TokenContract => SmartContract.SmartContract.ConvertFrom(_scs.CurrentChain!.Value.GetSmartContract(SmartContractType.ERC20Token));
		private SmartContract.SmartContract TrustContract => SmartContract.SmartContract.ConvertFrom(_scs.CurrentChain!.Value.GetSmartContract(SmartContractType.SecurityDeposit));

		private static readonly Dictionary<BigInteger, string> PublicRpc = new()
		{
			{ 1, "https://ethereum.publicnode.com" },
			{ 5, "https://ethereum-goerli.publicnode.com" },
			{ 11155111, "https://ethereum-sepolia.publicnode.com" },
		};

        private IWalletConnectionListener _connectionListener;

        public StandaloneExternalWalletCommunicator(StandaloneBrowserJsConfig standaloneBrowserJsConfig, SmartContractService scs)
		{
			_config = standaloneBrowserJsConfig;
			_scs = scs;
		}

		public async UniTask<ConnectionResponse> Connect(BigInteger chainId)
		{
			await UniTask.Delay(_config.ConnectionDelay);
			if (!_config.AcceptsConnection)
				throw new Exception("User declined the connection");
			InitializeWeb3IfNeeded(chainId);
			return new ConnectionResponse()
			{
				address = _account.Address,
				chainId = chainId.ToString()
			};
		}
		public async UniTask<string> GetBalance(string owner)
		{
			return await GetValue<BigInteger>(TokenContract, "balanceOf", owner);
		}
		public async UniTask<string> GetAllowance(string owner, string spender)
		{
			return await GetValue<BigInteger>(TokenContract, "allowance", owner, spender);
		}

		public async UniTask<string> ApproveMax(string owner, string spender, BigInteger value)
		{
			var parameters = new object[]
			{
				spender,
				value
			};
			var data = await GetFunctionCallData(TokenContract, "approve", parameters);
			return await SendTransaction(owner, TokenContract.Address, owner, data);
		}

		public async UniTask<bool> IsWalletAvailable()
		{
			await UniTask.Delay(_config.WalletAvailableDelay);
			return _config.IsWalletAvailable;
		}

		public async UniTask<string> SignMessage<TInput>(string address, TInput message)
		{
			await UniTask.Delay(_config.SignatureDelay);
			if (!_config.ShouldConfirmSignatures)
			{
				var reason = "ERROR:Signature Rejected by User";
				throw new Exception(reason);
			}

			var json = typeof(TInput) == typeof(string) ? (string)(object)message : JsonUtility.ToJson(message);
			var signature = _signer.SignTypedDataV4<Domain>(json, _key);
			return signature;
		}

        public void ExternalShowChainSelection()
        {
            Debug.Log("Show Chain Selection Window.");
        }
        public void ExternalShowConnectToWallet()
        {
            Debug.Log("Show Connect To Wallet Window.");
        }
        public void ExternalShowAccountInfo()
        {
            Debug.Log("Show Account Info Window.");
        }
        void IExternalWalletCommunicator.SetWalletConnectionListener(IWalletConnectionListener listener) => _connectionListener = listener;

        private async UniTask<string> SendSignedTransaction(string signedData)
		{
			if (!_config.ShouldSendTransactions)
				throw new Exception("ERROR:Transaction sending failed");
			if (!signedData.HasHexPrefix())
				signedData = "0x" + signedData;
			var transaction = await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signedData);
			var receipt = await _web3.TransactionReceiptPolling.PollForReceiptAsync(transaction);
			if (receipt.HasErrors() == true)
				throw new Exception($"ERROR:Transaction sending fail status {receipt.Status}, tx: {receipt.TransactionHash}");
			return receipt.TransactionHash;
		}

		private async UniTask<string> SignTransaction(string to, string from, string data)
		{
			if (!_config.ShouldSignTransactions)
				throw new Exception("ERROR:Transaction rejected by user");
			var input = new TransactionInput { To = to, From = from, Data = data };
			input.Gas = await _web3.TransactionManager.EstimateGasAsync(input);
			return await _web3.TransactionManager.SignTransactionAsync(input);
		}

		public async UniTask<string> SendTransaction(string address, string to, string from, string data)
		{
			var signedData = await SignTransaction(to, from, data);
			await UniTask.Yield();
			return await SendSignedTransaction(signedData);
		}
		public async UniTask<string> Deposit(string owner, string token, BigInteger value)
		{
			var parameters = new object[]
			{
				token,
				value
			};
			var data = await GetFunctionCallData(TrustContract, EncodeFunctionDataCalls.Deposit, parameters);
			return await SendTransaction(owner, TrustContract.Address, owner, data);
		}

		private async UniTask<string> GetValue<T>(SmartContract.SmartContract contract, string name, params string[] parameters)
		{
			await WaitForConfig();
			if (!_config.ShouldGetValues)
			{
				var reason = "ERROR:Does not work";
				return reason;
			}
			var nethereumContract = GetOrCreateContract(contract);
			var decimalsFunction = nethereumContract.GetFunction(name);
			T result;
			if (parameters != null
			    && parameters.Count() > 0)
				result = await decimalsFunction.CallAsync<T>(parameters);
			else
				result = await decimalsFunction.CallAsync<T>();
			return result.ToString();
		}

		public UniTask<string> GetFunctionCallData(SmartContract.SmartContract contract, string functionName, params object[] parameters)
		{
			var nethContract = GetOrCreateContract(contract);
			var nethFunction = nethContract.GetFunction(functionName);
			return UniTask.FromResult(nethFunction.GetData(parameters));
		}

		private void InitializeWeb3IfNeeded(BigInteger chainId)
		{
			if (_key is null
			    || _key.GetPrivateKey() != _config.GetWalletKey().GetPrivateKey())
				_account = null;
			if (_account != null)
				return;
			try
			{
				_key = _config.GetWalletKey();
				_account = new Account(_key, chainId);
				Debug.Log($"Local Eth account {_account.Address}\n Use this private key to top up metamask:\n{_account.PrivateKey}\n");
				_web3 = new Web3(_account, PublicRpc[chainId]);
				// MetamaskHostProvider host = MetamaskWebglHostProvider.CreateOrGetCurrentInstance();
				// _web3 = await host.GetWeb3Async();
				_signer = new Eip712TypedDataSigner();
				_contracts = new Dictionary<string, Contract>();
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to initialize web3 account. Check if the private key is valid.\n{e}");
			}
		}

		private Contract GetOrCreateContract(SmartContract.SmartContract contract)
		{
			if (string.IsNullOrEmpty(contract.Address))
				throw new Exception("Contract not initialized!!!");
			var address = contract.Address;
			if (_contracts.TryGetValue(address, out var createContract))
				return createContract;

			var nethereumContract = _web3.Eth.GetContract(contract.ABI, address);
			_contracts[address] = nethereumContract;
			return nethereumContract;
		}

		public async UniTask<string> SendDummyMessage<TInput>(TInput message)
		{
			await UniTask.Delay(1000);
			var toSerialize = new Message<TInput>()
			{
				ticket = 0,
				type = "Dummy",
				parameters = new Parameters<TInput>()
				{
					address = "address",
					message = message
				}
			};
			var serialized = JsonUtility.ToJson(toSerialize);
			Debug.Log($"Sending serialized message {serialized}");
			return serialized;
		}

		private async UniTask WaitForConfig()
		{
			await UniTask.WaitUntil(() => _scs.CurrentChain != null);
		}

		public void Dispose()
		{
		}
	}
}
