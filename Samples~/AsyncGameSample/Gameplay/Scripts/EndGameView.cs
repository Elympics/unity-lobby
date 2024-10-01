using Elympics;
using ElympicsLobbyPackage.Sample.AsyncGame;
using TMPro;
using UnityEngine;
using Elympics.Models.Authentication;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class EndGameView : MonoBehaviour
{
    private readonly string formattedRespectMessage = "You got {0} respect";

    [SerializeField] private int lobbySceneIndex = 0;
    [SerializeField] private TextMeshProUGUI respectText;

    public void Show()
    {
        gameObject.SetActive(true);
        DisplayRespect().Forget();
    }

    [UsedImplicitly] // by BackToLobbyButton
    public void ReturnToLobby()
    {
        SceneManager.LoadScene(lobbySceneIndex);
    }

    private async UniTask DisplayRespect()
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

            respectText.text = string.Format(formattedRespectMessage, respectValue.Respect.ToString());
        }
    }
}
