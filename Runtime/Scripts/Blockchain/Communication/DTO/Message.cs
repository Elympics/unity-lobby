using System;
namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
	[Serializable]
	public class Message<T>
	{
		public string protocolVersion;
		public int ticket;
		public string type;
		public Parameters<T> parameters;
	}
	[Serializable]
	public class Parameters<T>
	{
		public string address;
		public T message;
	}
}
