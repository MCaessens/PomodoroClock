using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Variables & Properties

    public static AudioManager Instance;
    [SerializeField] private GameObject audioPrefab;
    [SerializeField] private List<AudioItem> audioItems;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Public Methods

    public void StartEndSounds(float delayInSeconds, PomodoroManager.State state)
    {
        StartCoroutine(PlayEndSounds(delayInSeconds, state));
    }

    #endregion

    #region Private Methods

    private IEnumerator PlayEndSounds(float delayInSeconds, PomodoroManager.State state)
    {
        var clockItem = audioItems.FirstOrDefault(item => item.name.Contains("Clock"));
        audioPrefab.GetComponent<AudioSource>().clip = clockItem.clip;
        var clockSoundObj = Instantiate(audioPrefab);
        Destroy(clockSoundObj, delayInSeconds);
        
        yield return new WaitForSeconds(delayInSeconds);

        var focusItem = audioItems.FirstOrDefault(item => item.name.Contains("Focus"));
        var breakItem = audioItems.FirstOrDefault(item => item.name.Contains("Break"));
        var selectedItem = state == PomodoroManager.State.Focus ? focusItem : breakItem;
        
        audioPrefab.GetComponent<AudioSource>().clip = selectedItem.clip;
        audioPrefab.GetComponent<AudioSource>().volume = selectedItem.volume;
        var timeEndSound = Instantiate(audioPrefab);
        var endAudioSourceComponent = timeEndSound.GetComponent<AudioSource>();

        Destroy(timeEndSound, endAudioSourceComponent.clip.length);
    }

    #endregion
}
