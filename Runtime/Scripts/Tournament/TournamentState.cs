using JetBrains.Annotations;
namespace ElympicsLobbyPackage.Tournament
{
    [PublicAPI]
    public enum TournamentState
    {
        Created = 0,
        EventSent = 1,
        Settled = 2,
    }
}
