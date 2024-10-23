using Cysharp.Threading.Tasks;
using Elympics;
using Elympics.Models.Authentication;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    public class DefaultElympicsLobbyWrapper : MonoBehaviour, IElympicsLobbyWrapper
    {
        public AuthData AuthData => ElympicsLobbyClient.Instance!.AuthData;
        public bool IsAuthenticated => ElympicsLobbyClient.Instance!.IsAuthenticated;
        public IWebSocketSession WebSocketSession => ElympicsLobbyClient.Instance!.WebSocketSession;
        public void SignOut() => ElympicsLobbyClient.Instance!.SignOut();
        public UniTask ConnectToElympicsAsync(ConnectionData data) => ElympicsLobbyClient.Instance!.ConnectToElympicsAsync(data);
    }
}
