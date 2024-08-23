using UnityEngine;

namespace ElympicsLobbyPackage.Authorization
{
	[CreateAssetMenu(fileName = "StandaloneExternalAuthorizerConfig", menuName = "Configs/Authorizer/Standalone")]
	public class StandaloneExternalAuthorizerConfig : ScriptableObject
	{
		public Capabilities Capabilities => capabilities;
        public string ClosestRegion => closestRegion;

		[SerializeField] private Capabilities capabilities;
        [SerializeField] private string closestRegion;
    }
}
