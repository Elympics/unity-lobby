using System;
namespace ElympicsLobbyPackage.ExternalCommunication
{
	public interface IExternalGameStatusCommunicator: IDisposable
	{
		public void GameFinished(int score);

        public void RttUpdated(TimeSpan rtt);
		public void ApplicationInitialized();
	}
}
