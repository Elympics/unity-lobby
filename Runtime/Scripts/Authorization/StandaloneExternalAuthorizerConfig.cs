using UnityEngine;

namespace ElympicsLobbyPackage.Authorization
{
	[CreateAssetMenu(fileName = "StandaloneExternalAuthorizerConfig", menuName = "Configs/Authorizer/Standalone")]
	public class StandaloneExternalAuthorizerConfig : ScriptableObject
	{
		public Capabilities Capabilities => capabilities;

		[SerializeField] private Capabilities capabilities;
	}
}
