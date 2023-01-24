using System;
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
    
    [SerializeField] private TMP_InputField focusTimeInput;
    [SerializeField] private TMP_InputField breakTimeInput;
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
            var inputToParse = _currentState == State.Focus ? focusTimeInput.text : breakTimeInput.text;
            var isInputValid = float.TryParse(inputToParse, out var parsedTimeInMinutes);
            if (!isInputValid) return;

            parsedTimeInMinutes *= 60;
            timeGoalInSeconds = parsedTimeInMinutes;
            timeRemainingInSeconds = parsedTimeInMinutes;
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
        _previousState = State.Break;
        ObjectCleaner.RemoveAudioObjects();
        ResetToInitialState();
        HandleResetUI();
    }

    public void AddFiveMinutesToTimer()
    {
        timeRemainingInSeconds += 300f;
        timeGoalInSeconds += 300f;
    }

    #endregion

    #region Private Methods

    private void ResetToInitialState()
    {
        timeRemainingInSeconds = 0;
        timeGoalInSeconds = 0;
        _currentState = State.Idle;
        timeLabel.text = "00:00";
        _isEndTriggered = false;
    }

    private void EndTimer()
    {
        _previousState = _currentState;
        ResetToInitialState();

        HandleResetUI();
    }
    
    private void HandleStartUI()
    {
        focusTimeInput.interactable = false;
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
        stateLabel.text = "FOCUS TIME";
        startButton.interactable = true;
        pauseButton.interactable = false;
        focusTimeInput.interactable = true;
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
            EndTimer();
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