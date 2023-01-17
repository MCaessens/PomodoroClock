using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TimeBar : MonoBehaviour
{
    [SerializeField] private GameObject timeBarForGround;

    private void Awake()
    {
        DOTween.Init();
    }

    public void CalculateTimeBar(float timeRemainingInSeconds, float timeGoalInSeconds)
    {
        timeBarForGround.transform.localScale =
            new Vector3(timeRemainingInSeconds / timeGoalInSeconds, gameObject.transform.localScale.y);
    }

    public void ResetTimeBar()
    {
        timeBarForGround.transform.DOScale(new Vector3(1, 1), 1f);
    }
}