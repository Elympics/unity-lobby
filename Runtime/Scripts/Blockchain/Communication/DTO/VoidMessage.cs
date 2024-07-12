using System;
namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
	[Serializable]
	public class VoidMessage<T>
	{
		public string protocolVersion;
		public string type;
		public T message;
	}
}
