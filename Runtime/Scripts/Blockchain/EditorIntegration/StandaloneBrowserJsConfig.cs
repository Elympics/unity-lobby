#nullable enable
using System;
using ElympicsLobbyPackage.Utils;
using Nethereum.Signer;
using Plugins.Elympics.Plugins.ParrelSync;
using UnityEngine;

namespace ElympicsLobbyPackage.Blockchain.EditorIntegration
{
    [CreateAssetMenu(fileName = "StandaloneBrowserJsConfig", menuName = "Speculos/BrowserJsConfig")]
    public class StandaloneBrowserJsConfig : ScriptableObject
    {
        private const string EthPrivateKeyPlayerPrefsKeyBase = "Elympics/EthPrivateKey";
        private static string EthPrivateKeyPlayerPrefsKey =>
                ElympicsClonesManager.IsClone()
                ? $"{EthPrivateKeyPlayerPrefsKeyBase}_clone_{ElympicsClonesManager.GetCloneNumber()}"
                : EthPrivateKeyPlayerPrefsKeyBase;

        [Header("Wallet")]
        [SerializeField] private bool _isWalletAvailable;

        [Header("Connection")]
        [SerializeField] private bool _acceptsEagerConnection;
        [SerializeField] private bool _acceptsConnection;

        [Header("Signature")]
        [SerializeField] private bool _shouldConfirmSignatures;

        [Header("Blockchain")]
        [SerializeField] private bool _shouldSignTransactions;
        [SerializeField] private bool _shouldSendTransactions;
        [SerializeField] private bool _shouldGetValues;

        [Header("Delay")]
        [SerializeField] private int _walletAvailableDelay;
        [SerializeField] private int _connectionDelay;
        [SerializeField] private int _signatureDelay;

        private EthECKey _walletKey;

        public bool IsWalletAvailable => _isWalletAvailable;
        public bool AcceptsEagerConnection => _acceptsEagerConnection;
        public bool AcceptsConnection => _acceptsConnection;
        public bool ShouldConfirmSignatures => _shouldConfirmSignatures;
        public bool ShouldSignTransactions => _shouldSignTransactions;
        public bool ShouldSendTransactions => _shouldSendTransactions;
        public bool ShouldGetValues => _shouldGetValues;
        public int WalletAvailableDelay => _walletAvailableDelay;
        public int ConnectionDelay => _connectionDelay;
        public int SignatureDelay => _signatureDelay;

        public EthECKey? GetWalletKey()
        {
            if (_walletKey is null)
                ReinitializeUsedKey();
            return _walletKey;
        }

        private void ReinitializeUsedKey()
        {
            var args = Environment.GetCommandLineArgs();

            for (var i = 0; i < args.Length; i++)
            {
                if (!args[i].Contains("-privateKey"))
                    continue;
                try
                {
                    _walletKey = new EthECKey(args[i + 1]);
                    return;

                }
                catch (Exception)
                {
                    Debug.LogError($"Invalid privateKey from console arguments.");
                    break;
                }
            }

            var key = EthPrivateKeyPlayerPrefsKey;
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetString(key, CreateNewEthPrivateKey());
                PlayerPrefs.Save();
            }

            _walletKey = new EthECKey(PlayerPrefs.GetString(key));
        }

        private static string CreateNewEthPrivateKey() => EthECKey.GenerateKey().GetPrivateKey();


        private void InvokeRelogin()
        {
            //WalletWebGLEventsReceiver.WalletAccountsChanged();
        }
    }
}
