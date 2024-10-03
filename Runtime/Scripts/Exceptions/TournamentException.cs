using System;

namespace ElympicsLobbyPackage.Exceptions
{
    public class TournamentException : Exception
    {
        public TournamentException(string message) : base(message)
        {
        }
    }
}
