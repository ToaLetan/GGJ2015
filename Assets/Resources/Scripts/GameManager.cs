using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    private int[] playerScores = new int[2];

    private InputManager inputManager = null;

    public InputManager GameInputManager
    {
        get { return inputManager; }
    }

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;

        playerScores[0] = 0;
        playerScores[1] = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
        inputManager.Update();
	}

    public void AdjustScore(int playerNum, int scoreToAdd)
    {
        playerScores[playerNum - 1] += scoreToAdd;

        Debug.Log(playerScores[playerNum - 1]);
    }
}
