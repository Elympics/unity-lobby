using UnityEngine;
using System.Collections.Generic;
using Elympics;

namespace ElympicsLobbyPackage.Sample.AsyncGame
{
    public class GameManager : ElympicsMonoBehaviour, IUpdatable, IInitializable
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private MapManager mapManager;
        [SerializeField] private DisplayManager displayManager;
        [SerializeField] private Timer timer;
        [SerializeField] private ScoreManager scoreManager;

        public void Initialize()
        {
            if (Elympics.IsClient)
                PersistentLobbyManager.Instance?.SetAppState(PersistentLobbyManager.AppState.Gameplay);
        }

        public void ElympicsUpdate()
        {
            int readInput = 0;
            if (ElympicsBehaviour.TryGetInput(ElympicsPlayer.FromIndex(0), out IInputReader inputReader))
            {
                inputReader.Read(out readInput);
            }

            if (readInput != 0)
            {
                if (readInput == -1) playerManager.SetIsOnTheLeft(true);
                else if (readInput == 1) playerManager.SetIsOnTheLeft(false);
                displayManager.DislplayPlayer(playerManager.IsOnTheLeft);

                mapManager.UpdateRows((int)Elympics.Tick);
                var obstacleList = mapManager.GetObstacleList();
                displayManager.DisplayObstacles(obstacleList);

                if ((playerManager.IsOnTheLeft && obstacleList[0]) || (!playerManager.IsOnTheLeft && obstacleList[1]))
                {
                    EndGame();
                }
                else
                {
                    scoreManager.AddScore(1);
                }
            }

            timer.UpdateTimer();
            if (timer.IsOver) EndGame();
        }

        private void EndGame()
        {
            if (Elympics.IsServer) Elympics.EndGame(new ResultMatchPlayerDatas(new List<ResultMatchPlayerData> { new ResultMatchPlayerData { MatchmakerData = new float[1] { scoreManager.Score } } }));
            displayManager.ShowGameOver();
            if (Elympics.IsClient) ElympicsExternalCommunicator.Instance?.GameStatusCommunicator?.GameFinished(scoreManager.Score);
        }
    }
}
