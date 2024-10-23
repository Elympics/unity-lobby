using System;
namespace ElympicsLobbyPackage.ExternalCommunication
{
	public interface IExternalGameStatusCommunicator
	{
		public void GameFinished(int score);

        public void RttUpdated(TimeSpan rtt);
		public void ApplicationInitialized();
	}
}
