using System.Collections.Generic;
using UnityEngine;
using Elympics;

namespace ElympicsLobbyPackage.Sample.AsyncGame
{
    public class MapManager : MonoBehaviour, IInitializable
    {
        private ElympicsArray<ElympicsBool> obstacleList = null;

        [SerializeField] private int rowsCount = 5;
        [SerializeField] private int seed = 1;

        public void Initialize()
        {
            obstacleList = new ElympicsArray<ElympicsBool>(2 * rowsCount, () => new ElympicsBool(false));
        }

        private void MoveRows()
        {
            for (int i = 0; i < rowsCount - 1; i++)
            {
                obstacleList.Values[2 * i].Value = obstacleList.Values[2 * i + 2].Value;
                obstacleList.Values[2 * i + 1].Value = obstacleList.Values[2 * i + 3].Value;
            }
        }

        private void GenerateNewRow(int seedModifier)
        {
            var rng = new System.Random(seed + seedModifier);
            int option = rng.Next(0, 3);
            bool newLeft = option == 1;
            bool newRight = option == 2;

            obstacleList.Values[rowsCount * 2 - 2].Value = newLeft;
            obstacleList.Values[rowsCount * 2 - 1].Value = newRight;
        }

        public void UpdateRows(int seedModifier)
        {
            MoveRows();
            GenerateNewRow(seedModifier);
        }

        public List<bool> GetObstacleList()
        {
            List<bool> result = new List<bool>();
            foreach (ElympicsBool obstacle in obstacleList.Values) result.Add(obstacle.Value);
            return result;
        }
    }
}
