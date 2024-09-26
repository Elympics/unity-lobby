using System;

namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
    [Serializable]
    public class AuthDataChangedMessage
    {
        public string newJwt;
        public string newUserId;
        public string newNickname;
    }
}

