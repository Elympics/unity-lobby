using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    public static class LobbyPackageVersionRetriever
    {
        private const string ElympicsName = "elympicslobbypackage";

        public static Version GetVersionFromAssembly()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.Select(x => x.GetName()).FirstOrDefault(x => x.Name.ToLowerInvariant() == ElympicsName)?.Version;
        }

        public static string GetVersionStringFromAssembly()
        {
            return GetVersionFromAssembly()?.ToString(3) ?? string.Empty;
        }
    }
}
