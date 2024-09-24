using UnityEngine;
using ElympicsLobbyPackage.Blockchain.Wallet;
using ElympicsLobbyPackage.Session;
using Elympics;
using Cysharp.Threading.Tasks;
using System;

namespace ElympicsLobbyPackage.Sample.AsyncGame
{
    public class PersistentLobbyManager : MonoBehaviour
    {
        public enum AppState { Lobby, Matchmaking, Gameplay }

        private AuthorizationManager authorizationManager;
        private LobbyUIManager lobbyUIManager;

        private AppState appState = AppState.Lobby;
        public Guid CachedMatchId { get; private set; }

        public AppState CurrentAppState => appState;

        public static PersistentLobbyManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            ElympicsLobbyClient.Instance!.RoomsManager.MatchDataReceived += RememberMatchId;

            authorizationManager = FindObjectOfType<AuthorizationManager>();
            GameObject elympicsExternalCommunicator = ElympicsExternalCommunicator.Instance.gameObject;
            authorizationManager.InitializeAuthorizationManager(elympicsExternalCommunicator.GetComponent<SessionManager>(), elympicsExternalCommunicator.GetComponent<Web3Wallet>());
            SetLobbyUIManager();

            ElympicsExternalCommunicator.Instance.GameStatusCommunicator?.ApplicationInitialized();
            authorizationManager.AttemptStartAuthenticate().Forget();
        }

        public void SetAppState(AppState newState)
        {
            appState = newState;
            if (appState == AppState.Lobby)
            {
                SetLobbyUIManager();
                authorizationManager.AttemptReAuthenticate();
            }
        }

        private void SetLobbyUIManager()
        {
            lobbyUIManager = FindObjectOfType<LobbyUIManager>();
            lobbyUIManager.SetPersistantLobbyManager(this);
            authorizationManager.SetLobbyUIManager(lobbyUIManager);
        }

        private void RememberMatchId(MatchDataReceivedArgs obj)
        {
            CachedMatchId = obj.MatchId;
        }

        public void ConnectToWallet() => authorizationManager.ConnectToWallet();
    }
}
