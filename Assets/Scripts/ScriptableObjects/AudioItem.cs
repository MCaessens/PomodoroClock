using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AudioItem")]
public class AudioItem : ScriptableObject
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume;

    public PomodoroManager.State stateType;
}
