using System;
using SCS.InternalModels.Player;
using UnityEngine.Serialization;
namespace ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO
{
    [Serializable]
    internal class TrustDepositTransactionFinishedWebMessage
    {
        public int status;
        public string errorMessage;
        public float increasedAmount;
        public CheckDepositResponse trustState;
    }
}
