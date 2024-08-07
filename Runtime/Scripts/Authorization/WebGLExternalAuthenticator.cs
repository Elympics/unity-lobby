using System;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using Cysharp.Threading.Tasks;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.JWT;
using UnityEngine;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    internal class WebGLExternalAuthenticator : IExternalAuthenticator
    {
        private readonly JsCommunicator _jsCommunicator;
        public WebGLExternalAuthenticator(JsCommunicator jsCommunicator) => _jsCommunicator = jsCommunicator;

        public async UniTask<ExternalAuthData> InitializationMessage(string gameId, string gameName, string versionName)
        {
            var message = new InitializationMessage()
            {
                gameId = gameId,
                gameName = gameName,
                versionName = versionName
            };
            var result = await _jsCommunicator.SendRequestMessage<InitializationMessage, InitializationResponse>(ReturnEventTypes.Handshake, message);
            var capabilities = (Capabilities)result.capabilities;
            var isMobile = result.device == "mobile";
            if (result.error is not null)
            {
                Debug.LogError($"{nameof(WebGLExternalAuthenticator)} Error from initialization: {result.error}. Using standard authorization.");
                return new ExternalAuthData(null, isMobile, capabilities, result.environment);
            }
            if (!string.IsNullOrEmpty(result.authData.jwt))
            {
                var payload = JsonWebToken.Decode(result.authData.jwt, string.Empty, false);
                if (payload is null)
                {
                    Debug.LogError($"{nameof(WebGLExternalAuthenticator)} Payload is null. Something is wrong.");
                    return new ExternalAuthData(null, isMobile, capabilities, result.environment);
                }
                var formattedPayload = AuthTypeRaw.ToUnityNaming(payload);
                var payloadDeserialized = JsonUtility.FromJson<UnityPayload>(formattedPayload);
                if (string.IsNullOrEmpty(payloadDeserialized.authType) is false)
                {
                    var authType = AuthTypeRaw.ConvertToAuthType(payloadDeserialized.authType);
                    var cached = new AuthData(Guid.Parse(result.authData.userId), result.authData.jwt, result.authData.nickname, authType);
                    Debug.Log($"{nameof(WebGLExternalAuthenticator)} External authentication result: AuthType: {authType} UserId: {result.authData.userId} NickName: {result.authData.nickname}.");
                    return new ExternalAuthData(cached, isMobile, capabilities, result.environment);
                }
                Debug.LogError($"{nameof(WebGLExternalAuthenticator)} Couldn't find authType in payload.");
            }
            Debug.Log($"{nameof(WebGLExternalAuthenticator)} External message did not return authorization token. Using sdk to authenticate user.");
            return new ExternalAuthData(null, isMobile, capabilities, result.environment);
        }
    }
}
