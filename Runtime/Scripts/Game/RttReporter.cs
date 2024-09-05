using System;
using Elympics;
using ElympicsLobbyPackage;
using UnityEngine;

public class RttReporter : MonoBehaviour
{
    [SerializeField] private int tickInterval;
    private ElympicsClient _client;
    private int _counter;
    private void Awake() => _client = FindObjectOfType<ElympicsClient>();

    public void Update()
    {
        ++_counter;
        if (_counter <= tickInterval)
            return;

        _counter = 0;
        var avgRtt =_client.RoundTripTimeCalculator.AverageRoundTripTime;
        ElympicsExternalCommunicator.Instance.GameStatusCommunicator.RttUpdated(avgRtt);
    }
}
