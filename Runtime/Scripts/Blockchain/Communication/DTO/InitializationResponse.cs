#nullable enable
using System;
using UnityEngine.Serialization;
namespace ElympicsLobbyPackage.Blockchain.Communication.DTO
{
	[Serializable]
	public class InitializationResponse
	{
		public AuthDataRaw authData;
		public string? error;
		public string device;
		public string environment;
		public int capabilities;
	}

	[Serializable]
	public struct AuthDataRaw
	{
		public string jwt;
		public string userId;
		public string nickname;
	}
}
