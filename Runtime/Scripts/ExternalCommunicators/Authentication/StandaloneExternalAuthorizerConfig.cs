using System;
using System.Collections.Generic;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage.Tournament;
using UnityEngine;

namespace ElympicsLobbyPackage.Authorization
{
    [CreateAssetMenu(fileName = "StandaloneExternalAuthorizerConfig", menuName = "Configs/Standalone/Authorizer")]
    public class StandaloneExternalAuthorizerConfig : ScriptableObject
    {
        public AuthType AuthType => authType;
        public Capabilities Capabilities => capabilities;
        public string ClosestRegion => closestRegion;


        [SerializeField] private AuthType authType;
        [SerializeField] private Capabilities capabilities;
        [SerializeField] private string closestRegion;
    }
}
