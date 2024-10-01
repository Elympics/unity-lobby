using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElympicsLobbyPackage.Authorization
{
    [CreateAssetMenu(fileName = "StandaloneExternalAuthorizerConfig", menuName = "Configs/Authorizer/Standalone")]
    public class StandaloneExternalAuthorizerConfig : ScriptableObject
    {
        public Capabilities Capabilities => capabilities;
        public string ClosestRegion => closestRegion;
        public bool IsTournamentAvailable => isTournamentAvailable;
        public string TournamentName => tournamentName;
        public string OwnerId => ownerId;
        public int TournamentState => tournamentState;
        public string CreatedDate => createdDate;
        public string StartDate => startDate;
        public string EndDate => endDate;
        public List<string> Participants => participants;


        [SerializeField] private Capabilities capabilities;
        [SerializeField] private string closestRegion;

        [Header("Tournament")]
        [SerializeField] private bool isTournamentAvailable;
        [SerializeField] private string ownerId;
        [SerializeField] private string tournamentName;
        [SerializeField] private string createdDate;
        [SerializeField] private string startDate;
        [SerializeField] private string endDate;
        [SerializeField] private int tournamentState;
        [SerializeField] private List<string> participants;
    }
}
