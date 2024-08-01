using UnityEngine;
namespace Utils
{
	internal static class EnvironmentNames
	{
		public const string Production = "Prod";
		public const string Staging = "Stg";
		public const string Development = "Dev";

		public static string DefaultEnvironmentCheck()
		{
			Debug.Log("Using Default Environment check.");
			return Debug.isDebugBuild ? Development : Production;
		}
	}
}
