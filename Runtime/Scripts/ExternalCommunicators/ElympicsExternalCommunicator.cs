#nullable enable
using System;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.EditorIntegration;
using ElympicsLobbyPackage.ExternalCommunication;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.ExternalCommunication.Leaderboard;
using ElympicsLobbyPackage.ExternalCommunication.Tournament;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators;
using JetBrains.Annotations;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using Elympics;
using ElympicsLobbyPackage.Blockchain;
using ElympicsLobbyPackage.Blockchain.EditorIntegration;
#endif

namespace ElympicsLobbyPackage
{
    [RequireComponent(typeof(JsCommunicator))]
    [DefaultExecutionOrder(ElympicsLobbyExecutionOrders.ExternalCommunicator)]
    public class ElympicsExternalCommunicator : MonoBehaviour, IPlayPadEventListener
    {
        [PublicAPI]
        public event Action<string, string>? WalletConnected;

        [PublicAPI]
        public event Action? WalletDisconnected;

        [PublicAPI]
        public event Action<TrustDepositInfo>? TrustDepositCompleted;

        [PublicAPI]
        public static ElympicsExternalCommunicator? Instance;

        [PublicAPI]
        public IExternalAuthenticator? ExternalAuthenticator;

        [PublicAPI]
        public IExternalWalletCommunicator? WalletCommunicator;

        [PublicAPI]
        public IExternalGameStatusCommunicator? GameStatusCommunicator;

        [PublicAPI]
        public IExternalERC20SmartContractOperations? TokenCommunicator;

        [PublicAPI]
        public IExternalTrustSmartContractOperations? TrustCommunicator;

        [PublicAPI]
        public IExternalTournamentCommunicator? TournamentCommunicator;

        [PublicAPI]
        public IExternalLeaderboardCommunicator? LeaderboardCommunicator;

        [SerializeField] private StandaloneExternalAuthorizerConfig standaloneAuthConfig = null!;
        [SerializeField] private StandaloneExternalTournamentConfig standaloneTournamentConfig = null!;
        [SerializeField] private StandaloneBrowserJsConfig standaloneWalletConfig = null!;

        private JsCommunicator _jsCommunicator = null!;
        private WebGLFunctionalities? _webGLFunctionalities;
        private IElympicsLobbyWrapper _lobby;
        private ISmartContractServiceWrapper? _scsWrapper;

        private void Awake()
        {
            if (Instance == null)
            {
                _jsCommunicator = GetComponent<JsCommunicator>();
                if (_jsCommunicator == null)
                    throw new ArgumentNullException(nameof(_jsCommunicator), $"Couldn't find {nameof(JsCommunicator)} component on gameObject {gameObject.name}");

                _lobby = GetComponent<IElympicsLobbyWrapper>();
                if (_lobby == null)
                    throw new ArgumentNullException(nameof(_jsCommunicator), $"Couldn't find {nameof(IElympicsLobbyWrapper)} component on gameObject {gameObject.name}");


                _scsWrapper = GetComponent<ISmartContractServiceWrapper>();

#if UNITY_WEBGL && !UNITY_EDITOR
                _webGLFunctionalities = new WebGLFunctionalities(_jsCommunicator);
                ExternalAuthenticator = new WebGLExternalAuthenticator(_jsCommunicator);
                WalletCommunicator = new WebGLExternalWalletCommunicator(_jsCommunicator, _scsWrapper);
                GameStatusCommunicator = new WebGLGameStatusCommunicator(_jsCommunicator);
                var webGLContractOperations = new WebGLExternalContractOperations(_jsCommunicator);
                TokenCommunicator = new Erc20SmartContractCommunicator(webGLContractOperations, WalletCommunicator);
                TrustCommunicator = new WebGlTrustSmartContractCommunicator(_jsCommunicator, _lobby);
                TournamentCommunicator = new WebGLTournamentCommunicator(_jsCommunicator);
                LeaderboardCommunicator = new WebGLLeaderboardCommunicator(_jsCommunicator);

#else
                var standaloneCommunicator = new StandaloneCommunicator(standaloneWalletConfig);
                ExternalAuthenticator = new StandaloneExternalAuthenticator(standaloneAuthConfig, standaloneTournamentConfig);
                WalletCommunicator = standaloneCommunicator;
                TokenCommunicator = new Erc20SmartContractCommunicator(standaloneCommunicator, standaloneCommunicator);
                TrustCommunicator = new StandardExternalTrustSmartContractOperations(_scsWrapper);
                GameStatusCommunicator = new StandaloneExternalGameStatusCommunicator();
                TournamentCommunicator = customTournamentCommunicator != null ? customTournamentCommunicator : new StandaloneTournamentCommunicator(standaloneTournamentConfig, _jsCommunicator);
                LeaderboardCommunicator = customLeaderboardCommunicator != null ? customLeaderboardCommunicator : new StandaloneLeaderboardCommunicator();
#endif
                Instance = this;
                WalletCommunicator.SetPlayPadEventListener(this);
            }
            else
                Destroy(gameObject);
        }

#if UNITY_EDITOR || !UNITY_WEBGL
        [Header("Optional. Work only on UnityEditor")]
        [SerializeField] private CustomStandaloneLeaderboardCommunicatorBase? customLeaderboardCommunicator;

        [SerializeField] private CustomStandaloneTournamentCommunicatorBase? customTournamentCommunicator;


        [PublicAPI]
        public void SetCustomExternalAuthenticator(IExternalAuthenticator customExternalAuthenticator) => ExternalAuthenticator = customExternalAuthenticator ?? throw new ArgumentNullException(nameof(customExternalAuthenticator));
        [PublicAPI]
        public void SetCustomExternalWalletCommunicator(IExternalWalletCommunicator customExternalWalletCommunicator) => WalletCommunicator = customExternalWalletCommunicator ?? throw new ArgumentNullException(nameof(customExternalWalletCommunicator));
        [PublicAPI]
        public void SetCustomExternalGameStatusCommunicator(IExternalGameStatusCommunicator customExternalGameStatusCommunicator) => GameStatusCommunicator = customExternalGameStatusCommunicator ?? throw new ArgumentNullException(nameof(customExternalGameStatusCommunicator));

        [PublicAPI]
        public void SetCustomERC20TokenCommunicator(IExternalERC20SmartContractOperations customExternalErc20SmartContractOperations) => TokenCommunicator = customExternalErc20SmartContractOperations ?? throw new ArgumentNullException(nameof(customExternalErc20SmartContractOperations));
        [PublicAPI]
        public void SetCustomTrustTokenCommunicator(IExternalTrustSmartContractOperations customExternalTrustSmartContractOperations) => TrustCommunicator = customExternalTrustSmartContractOperations ?? throw new ArgumentNullException(nameof(customExternalTrustSmartContractOperations));
#endif

        private void OnDestroy()
        {
            WalletCommunicator?.Dispose();
            _webGLFunctionalities?.Dispose();
        }
        public void OnWalletConnected(string address, string chainId) => WalletConnected?.Invoke(address, chainId);
        public void OnWalletDisconnected() => WalletDisconnected?.Invoke();
        void IPlayPadEventListener.OnTrustDepositFinished(TrustDepositTransactionFinishedWebMessage transaction)
        {
            var currentTrustState = new TrustState()
            {
                AvailableAmount = transaction.trustState.Available,
                TotalAmount = transaction.trustState.totalAmount,
            };
            if (transaction.status == 0)
            {
                var added = transaction.increasedAmount;
                TrustDepositCompleted?.Invoke(new TrustDepositInfo
                {
                    Added = added,
                    TrustState = currentTrustState
                });
            }
            else
            {
                TrustDepositCompleted?.Invoke(new TrustDepositInfo()
                {
                    Added = 0,
                    TrustState = currentTrustState
                });
            }
        }
    }
}
