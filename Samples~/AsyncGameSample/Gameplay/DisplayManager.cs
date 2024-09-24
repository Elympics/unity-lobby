using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Elympics.Models.Authentication;
using Elympics;

namespace ElympicsLobbyPackage.Sample.AsyncGame
{
    public class DisplayManager : MonoBehaviour
    {
        [SerializeField] private List<Image> mapTiles;
        [SerializeField] private RectTransform playerTransform;
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private TextMeshProUGUI respectText;
        [SerializeField] private string lobbySceneName = "AsyncGameLobbyScene";

        public void DislplayPlayer(bool playerOnTheLeft)
        {
            playerTransform.anchoredPosition = new Vector2(Mathf.Abs(playerTransform.anchoredPosition.x) * (playerOnTheLeft ? -1 : 1), playerTransform.anchoredPosition.y);
        }

        public void DisplayObstacles(List<bool> obstacleList)
        {
            for (int i = 0; i < mapTiles.Count; i++)
            {
                mapTiles[i].color = obstacleList[i] ? Color.red : Color.green;
            }
        }

        public void ShowGameOver()
        {
            gameOverScreen.SetActive(true);
            DisplayRespect();
        }

        public void ReturnToLobbyButtonOnClick()
        {
            ReturnToLobby().Forget();
        }

        private async UniTask ReturnToLobby()
        {
            await SceneManager.LoadSceneAsync(lobbySceneName);
            PersistentLobbyManager.Instance.SetAppState(PersistentLobbyManager.AppState.Lobby);
        }

        private async void DisplayRespect()
        {
            if (ElympicsLobbyClient.Instance == null
                   || !ElympicsLobbyClient.Instance.IsAuthenticated
                   || ElympicsLobbyClient.Instance.AuthData.AuthType is AuthType.None or AuthType.ClientSecret)
            {
                respectText.text = "Log in to earn respect";
            }
            else
            {
                var respectService = new RespectService(ElympicsLobbyClient.Instance, ElympicsConfig.Load());
                var matchId = FindObjectOfType<PersistentLobbyManager>().CachedMatchId;
                var respectValue = await respectService.GetRespectForMatch(matchId);

                respectText.text = "Respect earned: " + respectValue.Respect.ToString();
            }
        }
    }
}
