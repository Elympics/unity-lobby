#nullable enable
using System;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.EditorIntegration;
using ElympicsLobbyPackage.ExternalCommunication;
using JetBrains.Annotations;
using SCS;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    [RequireComponent(typeof(JsCommunicator))]
    [DefaultExecutionOrder(ExecutionOrders.ExternalCommunicator)]
    public class ElympicsExternalCommunicator : MonoBehaviour
    {
        [PublicAPI]
        public static ElympicsExternalCommunicator? Instance;

        [PublicAPI]
        public IExternalAuthenticator? ExternalAuthenticator;

        [PublicAPI]
        public IExternalWalletCommunicator? WalletCommunicator;

        [PublicAPI]
        public IExternalGameStatusCommunicator? GameStatusCommunicator;

        [SerializeField] private SmartContractServiceConfig scsConfig = null!;

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
        public void SetCustomExternalWalletCommunicator(IExternalWalletCommunicator customExternalWalletCommunicator) => WalletCommunicator = customExternalWalletCommunicator ?? throw new ArgumentNullException(nameof(customExternalWalletCommunicator));
        [PublicAPI]
        public void SetCustomExternalGameStatusCommunicator(IExternalGameStatusCommunicator customExternalGameStatusCommunicator) => GameStatusCommunicator = customExternalGameStatusCommunicator ?? throw new ArgumentNullException(nameof(customExternalGameStatusCommunicator));
#endif

        private void OnDestroy() => WalletCommunicator?.Dispose();
    }
}
