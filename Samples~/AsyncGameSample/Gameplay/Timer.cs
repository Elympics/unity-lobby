using UnityEngine;
using UnityEngine.UI;
using Elympics;

namespace ElympicsLobbyPackage.Sample.AsyncGame
{
    public class Timer : ElympicsMonoBehaviour, IInitializable
    {
        [SerializeField] private Slider timerSlider;
        [SerializeField] private float initialTimerValue;
        private ElympicsFloat timer = new ElympicsFloat();
        public bool IsOver => timer.Value == 0;
        public void Initialize()
        {
            timer.Value = initialTimerValue;
        }

        public void DisplayTimer(float timerFill)
        {
            timerSlider.value = timerFill;
        }

        public void UpdateTimer()
        {
            timer.Value = Mathf.Max(0, timer.Value - Elympics.TickDuration);
            DisplayTimer(timer.Value / initialTimerValue);
        }
    }
}
