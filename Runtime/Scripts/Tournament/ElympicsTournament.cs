using Elympics;
using JetBrains.Annotations;
using UnityEngine;

namespace ElympicsLobbyPackage.Tournament
{
    [DefaultExecutionOrder(ElympicsLobbyExecutionOrders.ElympicsTournament)]
    public class ElympicsTournament : MonoBehaviour
    {
        [PublicAPI]
        public static ElympicsTournament Instance = null!;

        private TournamentInfo? _tournamentInfo;

        public int EntriesLeft { get; private set; }

        public TournamentPlayState PlayState { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                ElympicsLogger.LogError($"{nameof(ElympicsTournament)} singleton already exist. Destroying duplicate on {gameObject.name}");
                Destroy(gameObject);
            }
        }

        public void Initialize(TournamentInfo tournament)
        {
            _tournamentInfo = tournament;
        }
    }
}
