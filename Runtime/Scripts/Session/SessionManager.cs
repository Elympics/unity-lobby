#nullable enable
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Elympics;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain.Communication.Exceptions;
using ElympicsLobbyPackage.Blockchain.Wallet;
using ElympicsLobbyPackage.DataStorage;
using ElympicsLobbyPackage.ExternalCommunication;
using ElympicsLobbyPackage.JWT;
using ElympicsLobbyPackage.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace ElympicsLobbyPackage.Session
{
    [RequireComponent(typeof(Web3Wallet))]
    [DefaultExecutionOrder(ExecutionOrders.SessionManager)]
    public class SessionManager : MonoBehaviour
    {
        [PublicAPI]
        public SessionInfo? CurrentSession { get; private set; }

        [SerializeField] private string fallbackRegion = ElympicsRegions.Warsaw;

        private static SessionManager? instance;
        private string _region = null!;
        private IElympicsLobbyWrapper _lobbyWrapper = null!;
        private Web3Wallet? _wallet;
        private readonly AuthDataStorage _authDataStorage = new();
        private static IExternalAuthenticator ExternalAuthenticator => ElympicsExternalCommunicator.Instance!.ExternalAuthenticator!;
        private WalletConnectionStatus? _walletConnectionUpdate;

        private void Start()
        {
            _lobbyWrapper = GetComponent<IElympicsLobbyWrapper>();
            _wallet = GetComponent<Web3Wallet>();
            _wallet.WalletConnectionUpdatedInternal += OnWalletConnectionUpdated;
        }

        [PublicAPI]
        public async UniTask AuthenticateFromExternalAndConnect()
        {
            if (instance == null)
            {
                try
                {
                    await TryCheckExternalAuthentication();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    instance = this;
                }
            }
            else
                Destroy(gameObject);

            if (_lobbyWrapper is { IsAuthenticated: true, WebSocketSession: { IsConnected: true } })
                return;

            Debug.Log($"[{nameof(SessionManager)}] Check player wallet connection.");

            try
            {
                var address = await CheckWalletConnection();
                await TryConnectToWalletOrAnonymous(address);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                await AnonymousAuthentication();
            }
        }
        private static async UniTask<string> FindClosestRegion()
        {
            try
            {
                var closestRegion = await ElympicsCloudPing.ChooseClosestRegion(ElympicsRegions.AllAvailableRegions);
                return closestRegion.Region;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return string.Empty;
            }
        }

        [PublicAPI]
        public async UniTask<bool> TryReAuthenticateIfWalletChanged()
        {
            if (instance == null)
                throw new Exception($"Please Initialize SessionManager using {nameof(AuthenticateFromExternalAndConnect)} method");

            if (IsWalletEligible() is false)
                return false;

            Debug.Log($"[{nameof(SessionManager)}] Check if wallet connection has changed.");
            switch (_walletConnectionUpdate)
            {
                case WalletConnectionStatus.Connected:
                    Debug.Log($"[{nameof(SessionManager)}] Already authenticated but wallet address has changed.");
                    _walletConnectionUpdate = null;
                    await WalletAuthentication();
                    return true;
                case WalletConnectionStatus.Disconnected:
                    Debug.Log($"[{nameof(SessionManager)}] Already authenticated but wallet has been disconnected.");
                    _walletConnectionUpdate = null;
                    await AnonymousAuthentication();
                    return true;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }

        private void OnWalletConnectionUpdated(WalletConnectionStatus status)
        {
            Debug.Log($"Wallet connection status changed to: {status}");
            _walletConnectionUpdate = status;
        }

        [PublicAPI]
        public async UniTask ConnectToWallet()
        {
            try
            {
                var address = await CheckWalletConnection();
                if (string.IsNullOrEmpty(address))
                    _wallet.ExternalShowConnectToWallet();
                else
                    await TryConnectToWalletOrAnonymous(address);
            }
            catch (ResponseException e)
            {
                _wallet.ExternalShowConnectToWallet();
            }
            catch (ChainIdMismatch chainIdMismatch)
            {
                Debug.Log($"[{nameof(SessionManager)}] Chain Mismatch. {chainIdMismatch}");
                _wallet.ExternalShowChainSelection();
            }
        }

        private async UniTask TryConnectToWalletOrAnonymous(string walletAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(walletAddress) is false)
                    await WalletAuthentication();
                else
                    await AnonymousAuthentication();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log($"[{nameof(SessionManager)}] Something went wrong. Going anonymous auth {e}");
                await AnonymousAuthentication();
            }
        }

        private async UniTask TryCheckExternalAuthentication()
        {
            Debug.Log($"{nameof(SessionManager)} Check external authentication.");
            var config = ElympicsConfig.LoadCurrentElympicsGameConfig();
            var gameName = config.GameName;
            var gameId = config.GameId;
            var versionName = config.GameVersion;
#if UNITY_EDITOR || !UNITY_WEBGL
            if (ExternalAuthenticator is null)
                throw new Exception($"Please provide custom external authorizer via {nameof(ElympicsExternalCommunicator.SetCustomExternalAuthenticator)}");
#endif
            var result = await ExternalAuthenticator.InitializationMessage(gameId, gameName, versionName);

            await SetClosestRegion(result.ClosestRegion);

            if (result.AuthData is not null)
            {
                await AuthWithCached(result.AuthData, false, result);
                return;
            }
            CurrentSession = new SessionInfo(null, null, null, result.Capabilities, result.Environment, result.IsMobile, _region!);
            Debug.Log($"{nameof(SessionManager)} External message did not return auth token. Using sdk to authenticate user.");
        }
        private async UniTask SetClosestRegion(string externalClosestRegion)
        {
            if (string.IsNullOrEmpty(externalClosestRegion))
            {
                Debug.LogWarning($"External closest region is null.");
                var closestRegion = await FindClosestRegion();
                if (string.IsNullOrEmpty(closestRegion))
                {
                    Debug.LogWarning("Custom region search failed to find closest region. Using fallback region.");
                    _region = fallbackRegion;
                }
                else
                    _region = closestRegion;
            }
            else
                _region = externalClosestRegion;

            Debug.Log($"Closest region is {_region}");
        }

        private async UniTask WalletAuthentication()
        {
            try
            {
                if (_lobbyWrapper.IsAuthenticated)
                {
                    _lobbyWrapper.SignOut();
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
                        Debug.Log("[SessionManager] found token was invalid. Forcing re-authentication.");
                        await AuthWithEth();
                        return;
                    }
                    var token = JsonConvert.DeserializeObject<ElympicsJwt>(tokenAsString);
                    var isValid = IsTokenValid(token);
                    var isMatching = IsTokenMatching(token, _wallet!.Address);
                    if (!isValid
                        || !isMatching)
                    {
                        Debug.Log("[SessionManager] found token was invalid. Forcing re-authentication.");
                        await AuthWithEth();
                        return;
                    }

                    Debug.Log("[SessionManager] found matching cached token. Reusing value confirmed!");
                    await AuthWithCached(savedAuthData, true, null);
                    SaveNewAuthData();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                await AnonymousAuthentication();
            }
            await UniTask.CompletedTask;
        }
        private void SaveNewAuthData()
        {
            var authData = _lobbyWrapper.AuthData;

            if (authData is null)
            {
                _authDataStorage.Clear();
            }
            else
            {
                _authDataStorage.Set(authData);
            }
        }

        private void OnWalletConnected(string publicAddress, string chainId)
        {
            Debug.Log($"On Wallet connected");
            if (publicAddress == _wallet.Address)
                return;
            _walletConnectionUpdate = WalletConnectionStatus.Connected;
        }

        private void OnWalletDisconnected()
        {
            Debug.Log($"On Wallet connected");
            _walletConnectionUpdate = WalletConnectionStatus.Disconnected;
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
                await _lobbyWrapper.ConnectToElympicsAsync(new ConnectionData()
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
                var isMobile = external?.IsMobile ?? CurrentSession!.Value.IsMobile;
                var closestRegion = external?.ClosestRegion ?? CurrentSession!.Value.ClosestRegion;
                CurrentSession = new SessionInfo(_lobbyWrapper.AuthData, accountWallet, signWallet, capa, enviro, isMobile, closestRegion);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Couldn't login using cached data. Logging as guest. Reason: {Environment.NewLine} {e.Message}");
                await AnonymousAuthentication();
            }
        }

        private async UniTask AuthWithEth()
        {
            try
            {
                Debug.Log($"[{nameof(SessionManager)}] EthAddress Auth.");
                await _lobbyWrapper.ConnectToElympicsAsync(new ConnectionData()
                {
                    AuthType = AuthType.EthAddress,
                    Region = new RegionData(_region)
                });
                if (_lobbyWrapper.IsAuthenticated)
                {
                    SaveNewAuthData();
                    CurrentSession = new SessionInfo(_lobbyWrapper.AuthData!, _wallet.Address, _wallet.Address, CurrentSession.Value.Capabilities, CurrentSession.Value.Environment, CurrentSession.Value.IsMobile, CurrentSession.Value.ClosestRegion);
                }
                else
                {
                    await AnonymousAuthentication();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Couldn't login using EthAddress. Logging as guest. Reason: {Environment.NewLine} {e.Message}");
                await AnonymousAuthentication();
            }
        }

        private async UniTask AnonymousAuthentication()
        {
            if (_lobbyWrapper.AuthData?.AuthType is AuthType.ClientSecret)
            {
                Debug.Log($"Already authenticated as {AuthType.ClientSecret}.");
                return;
            }
            try
            {
                if (_lobbyWrapper.IsAuthenticated)
                {
                    _lobbyWrapper.SignOut();
                }
                Debug.Log($"[{nameof(SessionManager)}] ClientSecret Auth.");
                await _lobbyWrapper.ConnectToElympicsAsync(new ConnectionData()
                {
                    AuthType = AuthType.ClientSecret,
                    Region = new RegionData(_region)
                });
                CurrentSession = new SessionInfo(_lobbyWrapper.AuthData, null, null, CurrentSession!.Value.Capabilities, CurrentSession.Value.Environment, CurrentSession.Value.IsMobile, CurrentSession.Value.ClosestRegion);
            }
            catch (Exception e)
            {
                CurrentSession = null;

                if (!_lobbyWrapper.IsAuthenticated)
                    throw;

                _lobbyWrapper.SignOut();
                ;
                throw;
            }
        }

        private async UniTask<string> CheckWalletConnection()
        {
            if (IsWalletEligible() is false)
                return string.Empty;

            try
            {
                return await _wallet!.ConnectWeb3();
            }
            catch (ResponseException e)
            {
                if (e.Code == 404)
                    return string.Empty;
            }
            return string.Empty;
        }

        private void OnDestroy()
        {
            if (_wallet != null)
                _wallet.WalletConnectionUpdatedInternal -= OnWalletConnectionUpdated;
        }
        private bool IsWalletEligible() => CurrentSession.HasValue && (CurrentSession.Value.Capabilities.IsEth() || CurrentSession.Value.Capabilities.IsTon());

        internal void Reset()
        {
            instance = null;
            CurrentSession = null;
            _authDataStorage.Clear();
            _lobbyWrapper.SignOut();
        }
    }
}
