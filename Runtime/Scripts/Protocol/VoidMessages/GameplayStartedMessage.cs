using System;

namespace ElympicsLobbyPackage
{
    [Serializable]
    public struct GameplayStartedMessage
    {
        public string matchId;
        public SystemInfoData systemInfo;
    }
}
