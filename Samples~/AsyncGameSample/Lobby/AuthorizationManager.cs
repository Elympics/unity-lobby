using Cysharp.Threading.Tasks;
using UnityEngine;
using ElympicsLobbyPackage.Blockchain.Wallet;
using ElympicsLobbyPackage.Session;
using Elympics;
using System;

namespace ElympicsLobbyPackage.Sample.AsyncGame
{
    public class AuthorizationManager : MonoBehaviour
    {
        private SessionManager sessionManager;
        private Web3Wallet web3Wallet;
        private LobbyUIManager lobbyUIManager;

        public bool StartAuthenticationFinished { get; private set; } = false;

        public void InitializeAuthorizationManager(SessionManager sessionManager, Web3Wallet web3Wallet)
        {
            this.sessionManager = sessionManager;
            this.web3Wallet = web3Wallet;
        }

        public void SetLobbyUIManager(LobbyUIManager lobbyUIManager)
        {
            this.lobbyUIManager = lobbyUIManager;
        }

        public async UniTask AttemptStartAuthenticate()
        {
            lobbyUIManager.SetAuthenticationScreenActive(true);
            if (!ElympicsLobbyClient.Instance.IsAuthenticated || !ElympicsLobbyClient.Instance.WebSocketSession.IsConnected)
            {
                await sessionManager.AuthenticateFromExternalAndConnect();
            }
            lobbyUIManager.SetAuthenticationScreenActive(false);

            lobbyUIManager.SetLobbyUIVariant(sessionManager);
            web3Wallet.WalletConnectionUpdated += ReactToAuthenticationChange;

            StartAuthenticationFinished = true;
        }

        public async void AttemptReAuthenticate()
        {
            lobbyUIManager.SetAuthenticationScreenActive(true);
            await sessionManager.TryReAuthenticateIfWalletChanged();
            lobbyUIManager.SetLobbyUIVariant(sessionManager);
            lobbyUIManager.SetAuthenticationScreenActive(false);
        }

        public void ReactToAuthenticationChange(WalletConnectionStatus status)
        {
            if (PersistentLobbyManager.Instance.CurrentAppState == PersistentLobbyManager.AppState.Lobby)
            {
                AttemptReAuthenticate();
            }
        }

        public async void ConnectToWallet()
        {
            try
            {
                await sessionManager.ConnectToWallet();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                AttemptReAuthenticate();
            }
        }
    }
}
