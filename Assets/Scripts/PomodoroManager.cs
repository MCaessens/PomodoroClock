using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PomodoroManager : MonoBehaviour
{
    #region Variables & Properties

    private enum State
    {
        Work,
        Focus,
        Idle
    }

    [SerializeField] private TMP_InputField timeText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TimeBar timeBar;

    [SerializeField] private float timeRemainingInSeconds;
    [SerializeField] private float timeGoalInSeconds;

    private bool _isFirstTime;
    private State _currentState;
    private State? _previousState;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _currentState = State.Idle;
        _previousState = null;
        _isFirstTime = true;
    }

    private void Update()
    {
        if (_currentState == State.Idle) return;

        UpdateRemainingTime();
    }

    #endregion

    #region Public Methods

    public void StartTimer()
    {
        if (timeRemainingInSeconds == 0)
        {
            var isInputValid = float.TryParse(timeText.text, out var timeInput);
            if (!isInputValid) return;

            timeInput *= 60f;
            timeRemainingInSeconds = timeInput;
            timeGoalInSeconds = timeInput;
        }

        HandleStartUI();
        _currentState = _previousState == State.Focus || _isFirstTime ? State.Work : State.Focus;
        if (_isFirstTime) _isFirstTime = false;
    }

    public void PauseTimer()
    {
        HandlePauseUI();

        _previousState = _currentState;
        _currentState = State.Idle;
    }

    public void ResetTimer()
    {
        HandleResetUI();

        _currentState = State.Idle;
        timeRemainingInSeconds = 0;
        timeGoalInSeconds = 0;
    }

    #endregion

    #region Private Methods

    private void HandleStartUI()
    {
        timeText.interactable = false;
        startButton.interactable = false;
        pauseButton.interactable = true;
    }

    private void HandlePauseUI()
    {
        startButton.interactable = true;
        pauseButton.interactable = false;
    }

    private void HandleResetUI()
    {
        startButton.interactable = true;
        pauseButton.interactable = false;
        timeText.interactable = true;
        timeText.text = "";
    }

    private void UpdateRemainingTime()
    {
        if (timeRemainingInSeconds <= 0)
        {
            ResetTimer();
            return;
        }

        timeRemainingInSeconds -= Time.deltaTime;
        timeText.text = FormatTime(timeRemainingInSeconds);
        timeBar.CalculateTimeBar(timeRemainingInSeconds, timeGoalInSeconds);
    }

    private static string FormatTime(float timeInSeconds)
    {
        var stringBuilder = new StringBuilder();
        var timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        var minutes = timeSpan.Minutes;
        var seconds = timeSpan.Seconds;

        if (minutes == 0)
        {
            stringBuilder.Append(seconds);
            return stringBuilder.ToString();
        }

        stringBuilder.Append(AddNumberInsert(minutes));
        stringBuilder.Append($"{minutes}:");

        stringBuilder.Append(AddNumberInsert(seconds));
        stringBuilder.Append(seconds);

        return stringBuilder.ToString();
    }

    private static string AddNumberInsert(float number)
    {
        return number > 9 ? "" : "0";
    }

    private static string AddSemiColonInsert(float minutes, float seconds)
    {
        return minutes == 1 && seconds == 0 ? "" : ":";
    }

    #endregion
}