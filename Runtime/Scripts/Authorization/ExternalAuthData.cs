#nullable enable
using Elympics.Models.Authentication;
namespace ElympicsLobbyPackage.Authorization
{
	public struct ExternalAuthData
	{
		public readonly AuthData? AuthData;
		public readonly bool IsMobile;
		public readonly Capabilities Capabilities;
		public readonly string Environment;

		public ExternalAuthData(AuthData? authData, bool isMobile, Capabilities capabilities, string environment)
		{
			AuthData = authData;
			IsMobile = isMobile;
			Capabilities = capabilities;
			Environment = environment;
		}
	}
}
