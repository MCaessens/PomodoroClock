using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBar : MonoBehaviour
{
    [SerializeField] private GameObject timeBarForGround;

    public void CalculateTimeBar(float timeRemainingInSeconds, float timeGoalInSeconds)
    {
        timeBarForGround.transform.localScale =
            new Vector3(timeRemainingInSeconds / timeGoalInSeconds, gameObject.transform.localScale.y);
    }
}