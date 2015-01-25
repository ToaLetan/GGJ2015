using UnityEngine;
using System.Collections;

public class Timer
{
    private float currentTime = 0.0f;
    private float targetTime = 0.0f;

    private bool isTimerRunning = false;

    public delegate void TimerEvent();
    public event TimerEvent OnTimerStart;
    public event TimerEvent OnTimerComplete;

    public float CurrentTime
    {
        get { return currentTime; }
    }

    public float TargetTime
    {
        get { return targetTime; }
        set { targetTime = value; }
    }

    public bool IsTimerRunning
    {
        get { return isTimerRunning; }
    }


	// Use this for initialization
	public Timer (float maxTime, bool startOnCreation = false)
    {
        targetTime = maxTime;

        if (startOnCreation)
            StartTimer();
    }
	
	// Update is called once per frame
	public void Update () 
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= targetTime)
            {
                isTimerRunning = false;

                if (OnTimerComplete != null)
                    OnTimerComplete();
            }
        }
	}

    public void StartTimer()
    {
        isTimerRunning = true;

        if (OnTimerStart != null)
            OnTimerStart();
    }

    public void ResetTimer(bool startOnReset = false)
    {
        isTimerRunning = false;
        currentTime = 0;

        if (startOnReset)
            StartTimer();
    }
}
