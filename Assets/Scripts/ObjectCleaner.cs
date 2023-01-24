using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCleaner : MonoBehaviour
{
    public static ObjectCleaner Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static void RemoveAudioObjects()
    {
        var audioObjects = GameObject.FindGameObjectsWithTag("Sound");
        if (audioObjects.Length == 0) return;
        foreach (var audioObject in audioObjects)
        {
            Destroy(audioObject);
        }
    }
}
