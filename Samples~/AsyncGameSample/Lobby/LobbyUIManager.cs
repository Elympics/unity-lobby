using UnityEngine;
using TMPro;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Session;
using ElympicsLobbyPackage;
using Elympics;
using JetBrains.Annotations;

namespace ElympicsLobbyPackage.Sample.AsyncGame
{
    public class LobbyUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNickname;
        [SerializeField] private TextMeshProUGUI playerEthAddress;

        [SerializeField] private TextMeshProUGUI playButtonText;

        [SerializeField] private GameObject connectWalletButton;
        [SerializeField] private GameObject authenticationInProgressScreen;
        [SerializeField] private GameObject matchmakingInProgressScreen;
        [SerializeField] private GameObject playerAvatar;

        private PersistentLobbyManager persistentLobbyManager = null;
        private LeaderboardManager leaderboardManager = null;

        private string playQueue = "training";
        private string leaderBoardQueue;

        public void SetPersistantLobbyManager(PersistentLobbyManager newValue) => persistentLobbyManager = newValue;

        public void SetAuthenticationScreenActive(bool newValue) => authenticationInProgressScreen.SetActive(newValue);

        public void SetLobbyUIVariant(SessionManager sessionManager)
        {
            Capabilities capabilities = sessionManager.CurrentSession.Value.Capabilities;
            var currentAuthType = ElympicsLobbyClient.Instance.AuthData.AuthType;
            bool isGuest = currentAuthType is AuthType.ClientSecret or AuthType.None;

            playButtonText.text = isGuest ? "Train now" : "Play now";
            playerAvatar.SetActive(!isGuest);
            playerNickname.gameObject.SetActive(!isGuest);
            if (!isGuest)
            {
                playerNickname.text = sessionManager.CurrentSession.Value.AuthData.Nickname;
                if (!capabilities.IsTelegram())
                {
                    playerEthAddress.text = sessionManager.CurrentSession.Value.SignWallet;
                }
            }
            playerEthAddress.gameObject.SetActive(!isGuest && !capabilities.IsTelegram());
            connectWalletButton.SetActive((capabilities.IsEth() || capabilities.IsTon()) && isGuest);

            playQueue = currentAuthType switch
            {
                AuthType.Telegram => "telegram",
                AuthType.EthAddress => "eth",
                _ => "training",
            };

            leaderBoardQueue = currentAuthType == AuthType.Telegram ? "telegram" : "eth";

            if (leaderboardManager == null)
                leaderboardManager = FindObjectOfType<LeaderboardManager>();
            leaderboardManager.Initialize(leaderBoardQueue);
            leaderboardManager.UpdateLeaderboard();
        }


        [UsedImplicitly]
        public void ConnectToWallet()
        {
            persistentLobbyManager.ConnectToWallet();
            authenticationInProgressScreen.SetActive(true);
        }

        [UsedImplicitly]
        public async void PlayGame()
        {
            persistentLobbyManager.SetAppState(PersistentLobbyManager.AppState.Matchmaking);
            matchmakingInProgressScreen.SetActive(true);
            await ElympicsLobbyClient.Instance.RoomsManager.StartQuickMatch(playQueue);
        }

        [UsedImplicitly]
        public void ShowAccountInfo()
        {
            ElympicsExternalCommunicator.Instance.WalletCommunicator.ExternalShowAccountInfo();
        }
    }
}
