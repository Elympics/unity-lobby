using Cysharp.Threading.Tasks;
using Elympics;
using Elympics.Models.Authentication;

namespace ElympicsLobbyPackage
{
    internal interface IElympicsLobbyWrapper
    {

        AuthData AuthData { get; }

        bool IsAuthenticated { get; }

        IWebSocketSession WebSocketSession { get; }

        void SignOut();

        UniTask ConnectToElympicsAsync(ConnectionData data);
    }
}
