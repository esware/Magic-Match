using UnityEngine;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;

public class LIFESAddCounter : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private static float _timeLeft;
    private float _totalTimeForRestLife = 15f * 60; 
    private bool _startTimer;
    private DateTime _templateTime;
    
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _totalTimeForRestLife = InitScript.Instance.totalTimeForRestLifeHours * 60 * 60 + InitScript.Instance.totalTimeForRestLifeMin * 60 + InitScript.Instance.totalTimeForRestLifeSec;

    }

    bool CheckPassedTime()
    {
        if (InitScript.DateOfExit == "" || InitScript.DateOfExit == default(DateTime).ToString())
            InitScript.DateOfExit = DateTime.Now.ToString();

        DateTime dateOfExit = DateTime.Parse(InitScript.DateOfExit);
        if (DateTime.Now.Subtract(dateOfExit).TotalSeconds > _totalTimeForRestLife * (InitScript.Instance.capOfLife - InitScript.Lifes))
        {
            InitScript.Instance.RestoreLifes();
            InitScript.RestLifeTimer = 0;
            return false;
		}
        else
        {
            TimeCount((float)DateTime.Now.Subtract(dateOfExit).TotalSeconds);
            return true;     
		}
    }

    void TimeCount(float tick)
    {
        if (InitScript.RestLifeTimer <= 0)
            ResetTimer();
        
        InitScript.RestLifeTimer -= tick;
        
        if (InitScript.RestLifeTimer <= 1 && InitScript.Lifes < InitScript.Instance.capOfLife)
        {
            InitScript.Instance.AddLife(1);
            ResetTimer();
        }

    }

    void ResetTimer()
    {
        InitScript.RestLifeTimer = _totalTimeForRestLife;
    }
    
    void Update()
    {
        if (!_startTimer && DateTime.Now.Subtract(DateTime.Now).Days == 0)
        {
            InitScript.DateOfRestLife = DateTime.Now;
            if (InitScript.Lifes < InitScript.Instance.capOfLife)
            {
                if (CheckPassedTime())
                    _startTimer = true;
            }
        }

        if (_startTimer)
            TimeCount(Time.deltaTime);

        if (gameObject.activeSelf)
        {
            if (InitScript.Lifes < InitScript.Instance.capOfLife)
            {
                if (InitScript.Instance.totalTimeForRestLifeHours > 0)
                {
                    int hours = Mathf.FloorToInt(InitScript.RestLifeTimer / 3600);
                    int minutes = Mathf.FloorToInt((InitScript.RestLifeTimer - hours * 3600) / 60);
                    int seconds = Mathf.FloorToInt((InitScript.RestLifeTimer - hours * 3600) - minutes * 60);

                    _text.enabled = true;
                    _text.text = "" + string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                }
                else
                {
                    int minutes = Mathf.FloorToInt(InitScript.RestLifeTimer / 60F);
                    int seconds = Mathf.FloorToInt(InitScript.RestLifeTimer - minutes * 60);

                    _text.enabled = true;
                    _text.text = "" + string.Format("{0:00}:{1:00}", minutes, seconds);

                }
                InitScript.TimeForReps = _text.text;
            }
            else
            {
                _text.text = "   Full";
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            InitScript.DateOfExit = DateTime.Now.ToString();
        }
        else
        {
            _startTimer = false;
        }
    }

    void OnEnable()
    {
        _startTimer = false;
    }
    
    void OnApplicationQuit()
    {
        InitScript.DateOfExit = DateTime.Now.ToString();
    }
}
