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

    public enum State
    {
        Focus,
        Break,
        Idle,
        Paused
    }

    [SerializeField] private TMP_InputField timeInput;
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private TextMeshProUGUI stateLabel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button addFiveMinButton;
    [SerializeField] private TimeBar timeBar;

    [SerializeField] private float timeRemainingInSeconds;
    [SerializeField] private float timeGoalInSeconds;

    private bool _isFirstTime;
    private bool _isEndTriggered;
    private State _currentState;
    private State _previousState;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _currentState = State.Idle;
        _previousState = State.Idle;
        _isFirstTime = true;
    }

    private void Update()
    {
        if (_currentState is State.Idle or State.Paused) return;

        UpdateRemainingTime();
    }

    #endregion

    #region Public Methods

    public void StartTimer()
    {
        if (_currentState == State.Paused)
        {
            _currentState = _previousState;
            HandleStartUI();
            return;
        }
        _currentState =  _previousState == State.Break || _isFirstTime ? State.Focus : State.Break;

        if (timeRemainingInSeconds == 0)
        {
            if (_currentState == State.Break)
            {
                timeRemainingInSeconds = 600f;
                timeGoalInSeconds = 600f;
            }
            else
            {
                var isInputValid = float.TryParse(this.timeInput.text, out var timeInput);
                if (!isInputValid) return;

                timeInput *= 60f;
                timeRemainingInSeconds = timeInput;
                timeGoalInSeconds = timeInput;
            }
        }

        HandleStartUI();
        if (_isFirstTime) _isFirstTime = false;
    }

    public void PauseTimer()
    {
        _previousState = _currentState;
        _currentState = State.Paused;
        
        HandlePauseUI();
    }

    public void ResetTimer()
    {
        _previousState = _currentState;
        _currentState = State.Idle;
        timeRemainingInSeconds = 0;
        timeGoalInSeconds = 0;
        timeLabel.text = "00:00";
        _isEndTriggered = false;

        HandleResetUI();
    }

    public void AddFiveMinutesToTimer()
    {
        timeRemainingInSeconds += 300f;
        timeGoalInSeconds += 300f;
    }

    #endregion

    #region Private Methods

    private void HandleStartUI()
    {
        timeInput.interactable = false;
        stateLabel.text = $"{_currentState.ToString().ToUpper()} TIME";
        startButton.interactable = false;
        pauseButton.interactable = true;
        addFiveMinButton.interactable = true;
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
        timeInput.interactable = true;
        addFiveMinButton.interactable = true;
        timeBar.ResetTimeBar();
    }

    private void UpdateRemainingTime()
    {
        if (timeRemainingInSeconds <= 7 && !_isEndTriggered)
        {
            _isEndTriggered = true;
            AudioManager.Instance.StartEndSounds(7, _currentState);
        }

        if (timeRemainingInSeconds <= 0)
        {
            ResetTimer();
            return;
        }

        timeRemainingInSeconds -= Time.deltaTime;
        timeLabel.text = FormatTime(timeRemainingInSeconds);
        timeBar.CalculateTimeBar(timeRemainingInSeconds, timeGoalInSeconds);
    }

    private static string FormatTime(float timeInSeconds)
    {
        var stringBuilder = new StringBuilder();
        var timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        var minutes = timeSpan.Minutes;
        var seconds = timeSpan.Seconds;
        
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

    #endregion
}