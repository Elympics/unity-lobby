using System;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    public interface IExternalAuthenticator : IDisposable
    {
        public UniTask<ExternalAuthData> InitializationMessage(string gameId, string gameName, string versionName, string sdkVersion, string lobbyPackageVersion);
        internal void SetPlayPadEventListener(IPlayPadEventListener listener);
    }
}
