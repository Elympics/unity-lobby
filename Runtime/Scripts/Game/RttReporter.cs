using Elympics;
using UnityEngine;

namespace ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Game
{
    public class RttReporter : MonoBehaviour
    {
        [SerializeField] private int tickInterval;
        private ElympicsClient _client;
        private int _counter;
        private void Start() => _client = FindObjectOfType<ElympicsClient>();
        public void Update()
        {
            ++_counter;
            if (_counter <= tickInterval)
                return;

            _counter = 0;
            var avgRtt = _client.RoundTripTimeCalculator.AverageRoundTripTime;
            Debug.Log($"Reporting {avgRtt}");
            if (ElympicsExternalCommunicator.Instance != null)
            {
                ElympicsExternalCommunicator.Instance.GameStatusCommunicator?.RttUpdated(avgRtt);
            }
        }
    }
}
