using System;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using Cysharp.Threading.Tasks;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.JWT;
using ElympicsLobbyPackage.Tournament;
using ElympicsLobbyPackage.Tournament.Util;
using UnityEngine;
using ElympicsLobbyPackage.Session;
using ElympicsLobbyPackage.Blockchain.Communication.Exceptions;
using ElympicsLobbyPackage.Utils;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    internal class WebGLExternalAuthenticator : IExternalAuthenticator
    {
        private readonly JsCommunicator _jsCommunicator;
        public WebGLExternalAuthenticator(JsCommunicator jsCommunicator) => _jsCommunicator = jsCommunicator;

        public async UniTask<ExternalAuthData> InitializationMessage(string gameId, string gameName, string versionName, string sdkVersion, string lobbyPackageVersion)
        {
            var message = new InitializationMessage
            {
                gameId = gameId,
                gameName = gameName,
                versionName = versionName,
                sdkVersion = sdkVersion,
                lobbyPackageVersion = lobbyPackageVersion,
                systemInfo = SystemInfoDataFactory.GetSystemInfoData(),
            };
            try
            {
                var result = await _jsCommunicator.SendRequestMessage<InitializationMessage, InitializationResponse>(ReturnEventTypes.Handshake, message);
                var capabilities = (Capabilities)result.capabilities;
                var isMobile = result.device == "mobile";
                var closestRegion = result.closestRegion;
                ThrowIfInvalidResponse(result);
                var payloadDeserialized = result.authData.jwt.ExtractUnityPayloadFromJwt();
                var authType = AuthTypeRaw.ConvertToAuthType(payloadDeserialized.authType);
                var cached = new AuthData(Guid.Parse(result.authData.userId), result.authData.jwt, result.authData.nickname, authType);
                Debug.Log($"{nameof(WebGLExternalAuthenticator)} External authentication result: AuthType: {authType} UserId: {result.authData.userId} NickName: {result.authData.nickname}.");
                return new ExternalAuthData(cached, isMobile, capabilities, result.environment, closestRegion, result.tournamentData.ToTournamentInfo());
            }
            catch (ResponseException e)
            {
                if (e.Code == RequestErrors.ExternalAuthFailed)
                    throw new SessionManagerFatalError(e.Message);

                throw;
            }
        }
        private void ThrowIfInvalidResponse(InitializationResponse result)
        {
            if (string.IsNullOrEmpty(result.authData.jwt))
                throw new SessionManagerFatalError("External message did not return authorization token. Unable to authorize.");
            var payload = JsonWebToken.Decode(result.authData.jwt, string.Empty, false);
            if (payload is null)
                throw new SessionManagerFatalError($"Payload is null. Unable to authorize.");
            var formattedPayload = AuthTypeRaw.ToUnityNaming(payload);
            var payloadDeserialized = JsonUtility.FromJson<UnityPayload>(formattedPayload);
            if (string.IsNullOrEmpty(payloadDeserialized.authType))
                throw new SessionManagerFatalError("Couldn't find authType in payload. Unable to authorize.");
        }
    }
}
