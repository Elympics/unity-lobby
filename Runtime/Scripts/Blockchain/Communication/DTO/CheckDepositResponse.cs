using System;
using System.Numerics;
namespace ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO
{
    [Serializable]
    internal class CheckDepositResponse
    {
        public float totalAmount;
        public float lockedPendingSettlement;
        public float lockedPendingWithdrawal;
        public float Available => totalAmount - lockedPendingSettlement - lockedPendingWithdrawal;
    }
}
