using UnityEngine;
using Elympics;
using TMPro;

namespace ElympicsLobbyPackage.Sample.AsyncGame
{
    public class ScoreManager : ElympicsMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreDisplay;
        [SerializeField] private TextMeshProUGUI finalScoreDisplay;
        [SerializeField] private string finalScorePrefix;
        private ElympicsInt score = new ElympicsInt();
        public int Score => score.Value;

        private void Awake()
        {
            score.ValueChanged += UpdateDisplay;
        }

        public void AddScore(int addedValue)
        {
            score.Value += addedValue;
        }

        private void UpdateDisplay(int oldValue, int newValue)
        {
            scoreDisplay.text = score.Value.ToString();
            finalScoreDisplay.text = finalScorePrefix + score.Value.ToString();
        }
    }
}
