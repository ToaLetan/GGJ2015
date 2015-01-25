using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    public const float ROUND_LENGTH = 90; //Rounds are 1 minute 30 seconds.

    private int[] playerScores = new int[2];

    private Timer gameTimer = null;

    private InputManager inputManager = null;

    public InputManager GameInputManager
    {
        get { return inputManager; }
    }

    public Timer GameTimer
    {
        get { return gameTimer; }
    }

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;

        playerScores[0] = 0;
        playerScores[1] = 0;

        gameTimer = new Timer(ROUND_LENGTH, true); //TODO: MAKE A "ROUND START!" THING BEFORE THIS RUNS
        gameTimer.OnTimerComplete += OnRoundOver;
	}
	
	// Update is called once per frame
	void Update () 
    {
        inputManager.Update();
        gameTimer.Update();
	}

    public void AdjustScore(int playerNum, int scoreToAdd)
    {
        playerScores[playerNum - 1] += scoreToAdd;

        Debug.Log(playerScores[playerNum - 1]);
    }

    private void OnRoundOver()
    {
        //Determine the winner here.
    }
}
