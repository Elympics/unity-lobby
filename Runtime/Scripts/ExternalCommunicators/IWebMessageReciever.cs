using System.Collections;
using System.Collections.Generic;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using UnityEngine;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    public interface IWebMessageReceiver
    {
        void OnWebMessage(WebMessageObject message);
    }
}
