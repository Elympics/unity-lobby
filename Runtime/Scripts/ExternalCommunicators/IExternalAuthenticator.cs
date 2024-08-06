using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Authorization;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    public interface IExternalAuthenticator
    {
        public UniTask<ExternalAuthData> InitializationMessage(string gameId, string gameName, string versionName);
    }
}
