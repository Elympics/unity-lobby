﻿using TMPro;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TMP_InputField pointsField;
    [SerializeField] private EndGameView gameEndedView;

    public void UpdateTimer(int remainingSeconds) => timer.text = remainingSeconds.ToString();

    // TODO: Add showing points to EndView, fix bug with points not being saved when time ends
    public void ShowGameEndedView() => gameEndedView.Show();

    public int Points => int.TryParse(pointsField.text, out int result) ? result : 0;
}
