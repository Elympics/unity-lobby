using System;

namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
	[Serializable]
	public class TransactionToSign
	{
		public string from;
		public string to;
		public string data;
	}
}
