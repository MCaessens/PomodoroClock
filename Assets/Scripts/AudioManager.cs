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

    private AudioSource audioPrefabSource;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        Instance = this;
        audioPrefabSource = this.audioPrefab.GetComponent<AudioSource>();
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
        audioPrefabSource.clip = clockItem.clip;
        var clockSoundObj = Instantiate(audioPrefab);
        Destroy(clockSoundObj, delayInSeconds);
        
        yield return new WaitForSeconds(delayInSeconds);

        var focusItem = audioItems.FirstOrDefault(item => item.name.Contains("Focus"));
        var breakItem = audioItems.FirstOrDefault(item => item.name.Contains("Break"));
        var selectedItem = state == PomodoroManager.State.Focus ? focusItem : breakItem;
        
        audioPrefabSource.clip = selectedItem.clip;
        audioPrefabSource.volume = selectedItem.volume;
        var timeEndSound = Instantiate(audioPrefab);

        Destroy(timeEndSound, audioPrefabSource.clip.length);
    }

    #endregion
}
