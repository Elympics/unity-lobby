using System.Collections;
using System.Collections.Generic;
using ElympicsLobbyPackage.Blockchain.Communication;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    internal static class ElympicsLobbyExecutionOrders
    {
        public const int JsCommunicator = -10050;
        public const int ExternalCommunicator = -10000;
        public const int SessionManager = -9975;
        public const int DefaultSmartContractServiceWrapper = -9960;
        public const int Web3Wallet = -9950;
        public const int ElympicsTournament = -9800;
    }
}
