#nullable enable
using System;

namespace ElympicsLobbyPackage.Authorization
{
	[Serializable]
	public class UnityPayload
	{
		public string? authType;
		public string? ethAddress;
	}
}
