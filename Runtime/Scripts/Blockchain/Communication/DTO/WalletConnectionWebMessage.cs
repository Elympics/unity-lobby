using System;

namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
	[Serializable]
	public class WalletConnectionWebMessage
	{
		public bool isConnected;
		public string address;
		public string chainId;
	}
}

