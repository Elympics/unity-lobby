#nullable enable
using System;
using Cysharp.Threading.Tasks;
using Elympics;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.Communication.Exceptions;
using ElympicsLobbyPackage.Blockchain.Wallet;
using ElympicsLobbyPackage.DataStorage;
using ElympicsLobbyPackage.JWT;
using ElympicsLobbyPackage.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace ElympicsLobbyPackage.Session
{
    public class SessionManager
    {
        [PublicAPI]
        public SessionInfo? CurrentSession { get; private set; }

        [PublicAPI]
        public bool IsMobile;

        private string? _region;
        private readonly ElympicsLobbyClient _lobby;
        private readonly Web3Wallet _wallet;
        private WalletConnectionStatus? _walletConnectionUpdate;
        private readonly AuthDataStorage _authDataStorage = new();
        private static bool isInitialized;
        private ElympicsExternalCommunicator _externalCommunicator;

        public SessionManager(ElympicsLobbyClient lobby, Web3Wallet wallet, ElympicsExternalCommunicator externalCommunicator)
        {
            _lobby = lobby;
            _wallet = wallet;
            _externalCommunicator = externalCommunicator;
            externalCommunicator!.WalletCommunicator!.WalletDisconnected += OnWalletConnected;
            externalCommunicator.WalletCommunicator.WalletDisconnected += OnWalletDisconnected;
        }

        [PublicAPI]
        public async UniTask InitializeAndAuthorize()
        {

            if (isInitialized is false)
            {
                var closestRegion = await ElympicsCloudPing.ChooseClosestRegion(ElympicsRegions.AllAvailableRegions);
                _region = closestRegion.Region;
                await TryCheckExternalAuthorization();
            }

            if (_lobby is { IsAuthenticated: true, WebSocketSession: { IsConnected: true } })
                return;

            await TryConnectToWallet();

            isInitialized = true;
        }

        [PublicAPI]
        public async UniTask TryConnectToWallet()
        {
            if (_walletConnectionUpdate.HasValue is false)
                await CheckWalletStatus();
            else
                await TryReAuthorizeIfWalletChanged();
        }

        [PublicAPI]
        public async UniTask<bool> TryReAuthorizeIfWalletChanged()
        {
            if (isInitialized is false)
                throw new Exception($"Please Initialize SessionManager using {nameof(InitializeAndAuthorize)} method");

            var canBeWallet = CurrentSession.HasValue && (CurrentSession.Value.Capabilities.IsEth() || CurrentSession.Value.Capabilities.IsTon());

            if (canBeWallet is false)
                return false;

            Debug.Log($"[{nameof(SessionManager)}] Check if wallet connection has changed.");
            switch (_walletConnectionUpdate)
            {
                case WalletConnectionStatus.Reconnected:
                case WalletConnectionStatus.Connected:
                    Debug.Log($"[{nameof(SessionManager)}] Already authenticated but wallet address has changed.");
                    _walletConnectionUpdate = null;
                    await AuthorizeWithWallet();
                    return true;
                case WalletConnectionStatus.Disconnected:
                    Debug.Log($"[{nameof(SessionManager)}] Already authenticated but wallet has been disconnected.");
                    _walletConnectionUpdate = null;
                    await AnonymousAuthorization();
                    return true;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }

        private async UniTask TryCheckExternalAuthorization()
        {
            Debug.Log($"{nameof(SessionManager)} Check external authorization.");
            var config = ElympicsConfig.LoadCurrentElympicsGameConfig();
            var gameName = config.GameName;
            var gameId = config.GameId;
            var versionName = config.GameVersion;
#if UNITY_EDITOR || !UNITY_WEBGL
            if (_externalCommunicator.ExternalAuthorizer is null)
                throw new Exception($"Please provide custom external authorizer via {nameof(ElympicsExternalCommunicator.SetCustomExternalAuthorizer)}");
#endif
            var result = await _externalCommunicator.ExternalAuthorizer!.InitializationMessage(gameId, gameName, versionName);
            IsMobile = result.IsMobile;
            if (result.AuthData is not null)
            {
                await AuthWithCached(result.AuthData, false, result);
                return;
            }
            CurrentSession = new SessionInfo(null, null, null, result.Capabilities, result.Environment);
            Debug.Log($"{nameof(SessionManager)} External message did not return authorization token. Using sdk to authenticate user.");
        }

        private async UniTask AuthorizeWithWallet()
        {
            try
            {
                if (CurrentSession is not null)
                {
                    _lobby.SignOut();
                }
                var savedAuthData = _authDataStorage.Get();
                if (savedAuthData == null
                    || savedAuthData.AuthType == AuthType.ClientSecret)
                {
                    await AuthWithEth();
                }
                else
                {
                    var tokenAsString = JsonWebToken.Decode(savedAuthData.JwtToken, "", false);
                    if (string.IsNullOrEmpty(tokenAsString))
                    {
                        Debug.Log("[Lobby] found token was invalid. Forcing re-authentication.");
                        await AuthWithEth();
                        return;
                    }
                    var token = JsonConvert.DeserializeObject<ElympicsJwt>(tokenAsString);
                    var isValid = IsTokenValid(token);
                    var isMatching = IsTokenMatching(token, _wallet!.Address);
                    if (!isValid
                        || !isMatching)
                    {
                        Debug.Log("[Lobby] found token was invalid. Forcing re-authentication.");
                        await AuthWithEth();
                        return;
                    }

                    Debug.Log("[Lobby] found matching cached token. Reusing value confirmed!");
                    await AuthWithCached(savedAuthData, true, null);
                    SaveNewAuthData();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                await AnonymousAuthorization();
            }
            await UniTask.CompletedTask;
        }
        private void SaveNewAuthData()
        {
            var authData = _lobby.AuthData;

            if (authData is null)
            {
                _authDataStorage.Clear();
            }
            else
            {
                _authDataStorage.Set(authData);
            }
        }

        private void OnWalletDisconnected()
        {
            Debug.Log($"On Wallet connected");
            _walletConnectionUpdate = WalletConnectionStatus.Disconnected;
        }
        private void OnWalletConnected()
        {
            Debug.Log($"On Wallet connected");
            _walletConnectionUpdate = WalletConnectionStatus.Connected;
        }

        private bool IsTokenValid(ElympicsJwt token)
        {
            var gameConfig = ElympicsConfig.LoadCurrentElympicsGameConfig();
            if (gameConfig == null) return false;
            return token.gameId == gameConfig.GameId && token.gameName == gameConfig.GameName && token.versionName == gameConfig.GameVersion && token.chainId == _wallet!.ChainId && token.expiry >= DateTime.Now.AddHours(-1);
        }

        private bool IsTokenMatching(ElympicsJwt token, string expectedAddress)
        {
            return token.EthAddress.ToLower() == expectedAddress.ToLower();
        }

        private async UniTask AuthWithCached(AuthData cachedData, bool autoRetry, ExternalAuthData? external)
        {
            try
            {
                Debug.Log($"CachedData is {cachedData.AuthType}.");
                await _lobby.ConnectToElympicsAsync(new ConnectionData()
                {
                    Region = new RegionData(_region),
                    AuthFromCacheData = new CachedAuthData(cachedData, autoRetry)
                });

                string? accountWallet = null;
                string? signWallet = null;

                var payload = JsonWebToken.Decode(cachedData.JwtToken, string.Empty, false);
                if (payload is null)
                {
                    throw new Exception("Couldn't decode payload form jwt token.");
                }
                var formattedPayload = AuthTypeRaw.ToUnityNaming(payload);
                var payloadDeserialized = JsonUtility.FromJson<UnityPayload>(formattedPayload);

                accountWallet = payloadDeserialized.ethAddress is not null ? payloadDeserialized.ethAddress : null;
                if (cachedData.AuthType.IsWallet())
                {
                    signWallet = accountWallet;
                }
                var capa = external?.Capabilities ?? CurrentSession!.Value.Capabilities;
                var enviro = external?.Environment ?? CurrentSession!.Value.Environment;
                CurrentSession = new SessionInfo(_lobby.AuthData, accountWallet, signWallet, capa, enviro);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                await AnonymousAuthorization();
            }
        }

        private async UniTask AuthWithEth()
        {
            try
            {
                Debug.Log($"[{nameof(SessionManager)}] EthAddress Auth.");
                await _lobby.ConnectToElympicsAsync(new ConnectionData()
                {
                    AuthType = AuthType.EthAddress,
                    Region = new RegionData(_region)
                });
                if (_lobby.IsAuthenticated)
                {
                    SaveNewAuthData();
                    CurrentSession = new SessionInfo(_lobby.AuthData!, _wallet.Address, _wallet.Address, CurrentSession.Value.Capabilities, CurrentSession.Value.Environment);
                }
                else
                {
                    await AnonymousAuthorization();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                await AnonymousAuthorization();
            }
        }

        private async UniTask AnonymousAuthorization()
        {
            if (_lobby.AuthData?.AuthType is AuthType.ClientSecret)
            {
                Debug.Log("We are already client.");
                return;
            }
            try
            {
                if (_lobby.IsAuthenticated)
                {
                    _lobby.SignOut();
                }
                Debug.Log($"[{nameof(SessionManager)}] ClientSecret Auth.");
                await _lobby.ConnectToElympicsAsync(new ConnectionData()
                {
                    AuthType = AuthType.ClientSecret,
                    Region = new RegionData(_region)
                });
                CurrentSession = new SessionInfo(_lobby.AuthData, null, null, CurrentSession!.Value.Capabilities, CurrentSession.Value.Environment);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async UniTask<string> CheckWalletConnection()
        {
            if (IsWalletEligible())
            {
                return await _wallet!.ConnectWeb3();
            }
            return string.Empty;
        }
        private bool IsWalletEligible() => CurrentSession.HasValue && (CurrentSession.Value.Capabilities.IsEth() || CurrentSession.Value.Capabilities.IsTon());

        private async UniTask CheckWalletStatus()
        {
            try
            {
                Debug.Log($"[{nameof(SessionManager)}] Check player wallet connection.");
                var address = await CheckWalletConnection();
                if (string.IsNullOrEmpty(address))
                    await AnonymousAuthorization();
                else
                    await AuthorizeWithWallet();
            }
            catch (ChainIdMismatch chainIdMismatch)
            {
                Debug.Log($"[{nameof(SessionManager)}] Chain Mismatch. {chainIdMismatch}");
                _externalCommunicator!.WalletCommunicator!.ExternalShowChainSelection();
            }
            catch (ResponseException e)
            {
                if (e.Code == 404)
                {
                    Debug.Log($"[{nameof(SessionManager)}] Not authenticated no wallet address was found. {e}");
                    await AnonymousAuthorization();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log($"[{nameof(SessionManager)}] Something went wrong. Going anonymous auth {e}");
                await AnonymousAuthorization();
            }
        }
    }
}
