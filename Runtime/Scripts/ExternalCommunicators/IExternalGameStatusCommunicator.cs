namespace ElympicsLobbyPackage.ExternalCommunication
{
	public interface IExternalGameStatusCommunicator
	{
		public void GameFinished(int score);
		public void ApplicationInitialized();
	}
}
