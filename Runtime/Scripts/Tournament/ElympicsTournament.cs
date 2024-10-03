#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Elympics;
using ElympicsLobbyPackage.Exceptions;
using ElympicsLobbyPackage.ExternalCommunication.Leaderboard;
using ElympicsLobbyPackage.ExternalCommunication.Tournament;
using ElympicsLobbyPackage.Tournament.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace ElympicsLobbyPackage.Tournament
{
    [DefaultExecutionOrder(ElympicsLobbyExecutionOrders.ElympicsTournament)]
    public class ElympicsTournament : MonoBehaviour, IElympicsTournament
    {
        [PublicAPI]
        public event Action? TournamentFinished;

        [PublicAPI]
        public event Action? TournamentStarted;

        [PublicAPI]
        public event Action? TournamentUpdated;

        [PublicAPI]
        public static ElympicsTournament Instance = null!;

        [PublicAPI]
        public int? EntriesLeft { get; private set; }

        [PublicAPI]
        public TournamentPlayState PlayState { get; private set; }

        [PublicAPI]
        public bool IsTournamentAvailable => _tournamentInfo is not null;

        [PublicAPI]
        public TournamentInfo TournamentInfo => _tournamentInfo ?? throw new TournamentException("No tournament is available");

        private TournamentInfo? _tournamentInfo;

        private IRoomsManager _roomsManager = null!;
        private IExternalTournamentCommunicator _externalTournamentCommunicator = null!;
        private IExternalLeaderboardCommunicator _externalLeaderboardCommunicator = null!;

        private CancellationTokenSource? _timerCts;

        private readonly Dictionary<string, string> _tournamentCustomMatchmakingData = new(1);

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (ElympicsExternalCommunicator.Instance!.LeaderboardCommunicator == null)
                    throw new ElympicsException($"{nameof(ElympicsTournament)} requires {nameof(ElympicsExternalCommunicator.Instance.LeaderboardCommunicator)}");
                _externalLeaderboardCommunicator = ElympicsExternalCommunicator.Instance!.LeaderboardCommunicator;

                if (ElympicsExternalCommunicator.Instance.TournamentCommunicator == null)
                    throw new ElympicsException($"{nameof(ElympicsTournament)} requires {nameof(ElympicsExternalCommunicator.Instance.TournamentCommunicator)}");
                _externalTournamentCommunicator = ElympicsExternalCommunicator.Instance.TournamentCommunicator;

                _roomsManager = ElympicsLobbyClient.Instance!.RoomsManager;
                _externalTournamentCommunicator.TournamentUpdated += OnTournamentUpdated;

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                ElympicsLogger.LogError($"{nameof(ElympicsTournament)} singleton already exist. Destroying duplicate on {gameObject.name}");
                Destroy(gameObject);
            }
        }

        [PublicAPI]
        public async UniTask Initialize(TournamentInfo tournament)
        {
            Debug.Log($"Initialize {nameof(ElympicsTournament)}.");
            _tournamentInfo = tournament;
            _tournamentCustomMatchmakingData.Clear();
            _tournamentCustomMatchmakingData.Add(TournamentConst.TournamentIdKey, tournament.Id);
            SetPlayState();
            StartTimer();
            SendEventsOnInitialization();
            Debug.Log($"Tournament Initialized: {tournament.ToString()}");
            await UniTask.CompletedTask;
        }

        [PublicAPI]
        public async UniTask<LeaderboardStatus> FetchTournamentLeaderboard(int? pageNumber, int? pageSize)
        {
            ThrowIfNoTournament();
            return await _externalLeaderboardCommunicator.FetchLeaderboard(_tournamentInfo.Value.Id, null, new LeaderboardTimeScope(_tournamentInfo.Value.StartDate, _tournamentInfo.Value.EndDate), pageNumber ?? 1, pageSize ?? _tournamentInfo.Value.LeaderboardCapacity, LeaderboardRequestType.Regular);
        }

        [PublicAPI]
        public async UniTask<float> FetchUserHighScore()
        {
            ThrowIfNoTournament();
            var result = await _externalLeaderboardCommunicator.FetchUserHighScore(_tournamentInfo.Value.Id, null, new LeaderboardTimeScope(_tournamentInfo.Value.StartDate, _tournamentInfo.Value.EndDate), 1, 1);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (result.Score != -1.0f)
                return result.Score;
            throw new TournamentException("High-score for user has not been found.");
        }

        [PublicAPI]
        public async UniTask<IRoom> FindTournamentMatch(string queueName)
        {
            if (PlayState is TournamentPlayState.NotStarted)
                throw new TournamentException("Tournament has not started.");

            if (PlayState is TournamentPlayState.Finished)
                throw new TournamentException("Tournament has finished.");

            if (EntriesLeft <= 0)
                throw new TournamentException("Not enough entries.");
            return await _roomsManager.StartQuickMatch(queueName, null, null, null, _tournamentCustomMatchmakingData);
        }

        private void SendEventsOnInitialization()
        {
            if (IsTournamentFinished())
                TournamentFinished?.Invoke();
            else if (IsTournamentOngoing())
                TournamentStarted?.Invoke();
        }

        private void OnTournamentUpdated(TournamentInfo newTournamentInfo)
        {
            Debug.Log($"Update tournament using new info: {newTournamentInfo.ToString()}.");
            Initialize(newTournamentInfo).ContinueWith(() => { TournamentUpdated?.Invoke(); }).Forget();
        }

        private void StartTimer()
        {
            if (_timerCts is not null)
            {
                _timerCts.Cancel();
                _timerCts.Dispose();
            }

            _timerCts = new CancellationTokenSource();

            ThrowIfNoTournament();

            if (IsTournamentUpcoming())
            {
                RequestTimer(_tournamentInfo.Value.StartDate,
                    () =>
                    {
                        Debug.Log("Timer: Tournament Started.");
                        TournamentStarted?.Invoke();
                        SetPlayState();
                        StartTimer();
                    },
                    _timerCts.Token).Forget();
            }
            else if (IsTournamentOngoing())
            {
                RequestTimer(_tournamentInfo.Value.EndDate,
                    () =>
                    {
                        Debug.Log("Timer: Tournament Finished.");
                        SetPlayState();
                        TournamentFinished?.Invoke();
                    },
                    _timerCts.Token).Forget();
            }
        }
        private bool IsTournamentUpcoming() => DateTimeOffset.Now < _tournamentInfo?.StartDate;
        private bool IsTournamentFinished() => DateTimeOffset.Now > _tournamentInfo?.EndDate;
        private bool IsTournamentOngoing() => _tournamentInfo?.StartDate <= DateTimeOffset.Now && DateTimeOffset.Now < _tournamentInfo?.EndDate;
        private void SetPlayState()
        {
            EntriesLeft = _tournamentInfo!.Value.TonDetails?.EntriesLeft;

            if (IsTournamentUpcoming())
                PlayState = TournamentPlayState.NotStarted;
            else if (IsTournamentFinished())
                PlayState = TournamentPlayState.Finished;
            else
                PlayState = EntriesLeft <= 0 ? TournamentPlayState.LackOfEntries : TournamentPlayState.Playable;

            Debug.Log($"Tournament PlayState is {PlayState}.");
        }
        private async UniTask RequestTimer(DateTimeOffset date, Action callback, CancellationToken ct = default)
        {
            Debug.Log($"Request timer for {date:O}");
            var isCanceled = await UniTask.WaitUntil(() => DateTimeOffset.Now > date, PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
            if (isCanceled)
            {
                Debug.Log("Timer has been canceled.");
                return;
            }
            callback.Invoke();
        }
        private void OnDestroy()
        {
            _externalTournamentCommunicator.TournamentUpdated -= OnTournamentUpdated;
            _timerCts?.Cancel();
        }
        internal async UniTask Initialize(TournamentInfo tournament, IRoomsManager roomsManager)
        {
            _tournamentInfo = tournament;
            _tournamentCustomMatchmakingData.Clear();
            _tournamentCustomMatchmakingData.Add(TournamentConst.TournamentIdKey, tournament.Id.ToString());
            _roomsManager = roomsManager;
            SetPlayState();
            StartTimer();
            await UniTask.CompletedTask;
        }

        private void ThrowIfNoTournament()
        {
            if (_tournamentInfo == null)
                throw new TournamentException("Tournament is not available.");
        }
    }
}
