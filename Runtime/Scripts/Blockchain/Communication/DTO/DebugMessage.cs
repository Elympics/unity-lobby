using System;
namespace ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO
{
    [Serializable]
    public class DebugMessage<T>
    {
        public string debugType;
        public T message;
    }
}
