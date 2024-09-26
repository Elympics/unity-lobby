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
        private AuthenticationManager authenticationManager;
        private LobbyUIManager lobbyUIManager;

        private bool isAuthChangePossible = true;
        public bool IsAuthChangePossible => isAuthChangePossible;

        public Guid CachedMatchId { get; private set; }

        public static PersistentLobbyManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            ElympicsLobbyClient.Instance!.RoomsManager.MatchDataReceived += RememberMatchId;

            authenticationManager = FindObjectOfType<AuthenticationManager>();
            GameObject elympicsExternalCommunicator = ElympicsExternalCommunicator.Instance.gameObject;
            authenticationManager.InitializeAuthenticationManager(elympicsExternalCommunicator.GetComponent<SessionManager>(), elympicsExternalCommunicator.GetComponent<Web3Wallet>());
            SetLobbyUIManager();

            ElympicsExternalCommunicator.Instance.GameStatusCommunicator?.ApplicationInitialized();
            authenticationManager.AttemptStartAuthenticate().Forget();
        }

        public void ChangeAuthAvialability(bool newState)
        {
            isAuthChangePossible = newState;
            if (isAuthChangePossible && authenticationManager.StartAuthenticationFinished)
            {
                SetLobbyUIManager();
                authenticationManager.AttemptReAuthenticate().Forget();
            }
        }

        private void SetLobbyUIManager()
        {
            lobbyUIManager = FindObjectOfType<LobbyUIManager>();
            authenticationManager.SetLobbyUIManager(lobbyUIManager);
        }

        private void RememberMatchId(MatchDataReceivedArgs obj)
        {
            CachedMatchId = obj.MatchId;
        }

        public void ConnectToWallet() => authenticationManager.ConnectToWallet().Forget();
    }
}
