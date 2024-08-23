using System;
using Cysharp.Threading.Tasks;
using Elympics;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage;
using UnityEngine;

namespace ElympicsLobby.Tests.PlayMode
{
    public class TestElympicsLobbyWrapperClient : MonoBehaviour, IElympicsLobbyWrapper
    {
        public AuthData AuthData { get; private set; }

        public bool IsAuthenticated => AuthData != null;

        public IWebSocketSession WebSocketSession
        {
            get
            {
                if (_mockWebSocket == null)
                {
                    _mockWebSocket = new MockWebSocket();
                    _mockWebSocket.ToggleConnection(false);
                }
                return _mockWebSocket;
            }
        }

        private MockWebSocket _mockWebSocket;
        public void SignOut()
        {
            AuthData = null;
            _mockWebSocket?.ToggleConnection(false);
        }
        public UniTask ConnectToElympicsAsync(ConnectionData data)
        {
            if (data.AuthType is not null)
            {
                AuthData = new AuthData(Guid.Empty, "", "nickName", data.AuthType.Value);
            }
            else
            {
                if (data.AuthFromCacheData is not null)
                {
                    AuthData = data.AuthFromCacheData.Value.CachedData;
                }
            }
            _mockWebSocket = new MockWebSocket();
            _mockWebSocket.ToggleConnection(true);
            return UniTask.CompletedTask;
        }
    }

    public class MockWebSocket : IWebSocketSession
    {
        public event Action Connected;
        public event Action<DisconnectionData> Disconnected;
        public bool IsConnected { get; private set; }

        public void ToggleConnection(bool connected) => IsConnected = connected;
    }
}
