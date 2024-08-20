#nullable enable
using System;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.EditorIntegration;
using ElympicsLobbyPackage.ExternalCommunication;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators;
using JetBrains.Annotations;
using SCS;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    [RequireComponent(typeof(JsCommunicator))]
    [DefaultExecutionOrder(ExecutionOrders.ExternalCommunicator)]
    public class ElympicsExternalCommunicator : MonoBehaviour, IWalletConnectionListener
    {
        [PublicAPI]
        public event Action<string, string>? WalletConnected;

        [PublicAPI]
        public event Action? WalletDisconnected;

        [PublicAPI]
        public static ElympicsExternalCommunicator? Instance;

        [PublicAPI]
        public IExternalAuthenticator? ExternalAuthenticator;

        [PublicAPI]
        public IExternalWalletCommunicator? WalletCommunicator;

        [PublicAPI]
        public IExternalGameStatusCommunicator? GameStatusCommunicator;

        [SerializeField] private SmartContractServiceConfig scsConfig = null!;

        [SerializeField] private StandaloneExternalAuthorizerConfig standaloneAuthConfig = null!;

        [SerializeField] private StandaloneBrowserJsConfig standaloneWalletConfig = null!;

        private JsCommunicator _jsCommunicator = null!;
        private void Awake()
        {
            if (Instance is null)
            {
                _jsCommunicator = GetComponent<JsCommunicator>();
                if (_jsCommunicator is null)
                    throw new ArgumentNullException(nameof(_jsCommunicator), $"Couldn't find JsCommunicator component on gameObject {gameObject.name}");
#if UNITY_WEBGL && !UNITY_EDITOR
            ExternalAuthenticator = new WebGLExternalAuthenticator(_jsCommunicator);
            WalletCommunicator = new WebGLExternalWalletCommunicator(_jsCommunicator, scsConfig);
            GameStatusCommunicator = new WebGLGameStatusCommunicator(_jsCommunicator);
#else
                ExternalAuthenticator = new StandaloneExternalAuthenticator(standaloneAuthConfig);
                WalletCommunicator = new StandaloneExternalWalletCommunicator(standaloneWalletConfig, GetComponent<SmartContractService>());
#endif
                Instance = this;
            }
            else
                Destroy(gameObject);
        }

#if UNITY_EDITOR || !UNITY_WEBGL
        [PublicAPI]
        public void SetCustomExternalAuthenticator(IExternalAuthenticator customExternalAuthenticator) => ExternalAuthenticator = customExternalAuthenticator ?? throw new ArgumentNullException(nameof(customExternalAuthenticator));
        [PublicAPI]
        public void SetCustomExternalWalletCommunicator(IExternalWalletCommunicator customExternalWalletCommunicator)
        {
            WalletCommunicator = customExternalWalletCommunicator ?? throw new ArgumentNullException(nameof(customExternalWalletCommunicator));
            WalletCommunicator.SetWalletConnectionListener(this);
        }
        [PublicAPI]
        public void SetCustomExternalGameStatusCommunicator(IExternalGameStatusCommunicator customExternalGameStatusCommunicator) => GameStatusCommunicator = customExternalGameStatusCommunicator ?? throw new ArgumentNullException(nameof(customExternalGameStatusCommunicator));
#endif

        private void OnDestroy() => WalletCommunicator?.Dispose();
        public void OnWalletConnected(string address, string chainId) => WalletConnected?.Invoke(address, chainId);
        public void OnWalletDisconnected() => WalletDisconnected?.Invoke();
    }
}
