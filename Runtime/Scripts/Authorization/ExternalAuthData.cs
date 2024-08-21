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
        public readonly string ClosestRegion;

		public ExternalAuthData(AuthData? authData, bool isMobile, Capabilities capabilities, string environment, string closestRegion)
		{
			AuthData = authData;
			IsMobile = isMobile;
			Capabilities = capabilities;
			Environment = environment;
            ClosestRegion = closestRegion;
        }
	}
}
