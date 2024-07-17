using System;

namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
	[Serializable]
	public class WalletConnectionMessage
	{
		public bool isConnected;
		public string address;
		public string chainId;
	}
}

