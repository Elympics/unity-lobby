using System;

namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
	[Serializable]
	internal class InitializationMessage
	{
		public string gameId;
		public string gameName;
		public string versionName;
        public string renderer;
        public string vendor;
    }
}
