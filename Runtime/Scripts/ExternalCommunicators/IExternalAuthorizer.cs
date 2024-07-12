using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Authorization;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    public interface IExternalAuthorizer
    {
        public UniTask<ExternalAuthData> InitializationMessage(string gameId, string gameName, string versionName);
    }
}
