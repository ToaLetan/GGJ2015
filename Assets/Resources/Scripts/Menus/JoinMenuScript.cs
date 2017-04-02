using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JoinMenuScript : MonoBehaviour 
{
    private Color hide = new Color(0, 0, 0, 0);

    private GameManager gameManager = null;
    private InputManager inputManager = null;

    private GameObject[] animatedPrompts = new GameObject[2];
    private GameObject[] okPrompts = new GameObject[2];

    private bool[] playerJoined = new bool[2];

    private GameObject startText = null;
    private GameObject startPrompt = null;

    private Color startActiveColour = new Color(0.08f, 0.03f, 0.09f);
    private Color startInactiveColour = new Color(0.54f, 0.46f, 0.3f);

    private int currentPlayerIndex = 0;

    private bool canStartGame = false;

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
        inputManager = InputManager.Instance;

        //Subscribe to Input Manager events for both keyboard and controller input (only need buttons)
        inputManager.Key_Pressed += ProcessKeys;
        inputManager.Button_Pressed += ProcessButtons;

        for (int i = 0; i < animatedPrompts.Length; i++)
        {
            animatedPrompts[i] = gameObject.transform.FindChild("Prompt_Anim" + (i + 1)).gameObject;

            okPrompts[i] = gameObject.transform.FindChild("Text_OK" + (i + 1)).gameObject;
            okPrompts[i].transform.GetComponent<SpriteRenderer>().enabled = false;

            playerJoined[i] = false;
        }

        startText = gameObject.transform.FindChild("Text_Start").gameObject;
        startText.GetComponent<SpriteRenderer>().color = startInactiveColour;
        startPrompt = gameObject.transform.FindChild("Prompt_Start").gameObject;
        startPrompt.GetComponent<SpriteRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void ProcessKeys(List<string> keysPressed)
    {
        if (currentPlayerIndex < 2)
        {
            for (int i = 0; i < inputManager.KeybindArray.Length; i++)
            {
                if (keysPressed.Contains(inputManager.KeybindArray[i].buttonAKey.ToString()))
                {
                    if (playerJoined[currentPlayerIndex] == false)
                    {
                        string inputString = inputManager.KeybindArray[i].ToString() + " " + i;

                        if (gameManager.PlayerInputSources.Contains(inputString) == false)
                            JoinPlayer(inputString);
                    }
                }
            }
        }
        else
        {
            if (keysPressed.Contains(inputManager.KeybindArray[0].startKey.ToString()) && canStartGame == true)
                ProceedToGameIntro();
        }
    }

    private void ProcessButtons(List<string> buttonsPressed)
    {
        if (currentPlayerIndex <= 2)
        {
            for (int i = 0; i < inputManager.ControllerArray.Length; i++)
            {
                if (buttonsPressed.Contains(inputManager.ControllerArray[i].buttonA))
                {
                    if (playerJoined[currentPlayerIndex] == false)
                    {
                        string inputString = inputManager.ControllerArray[i].ToString() + " " + i;

                        if (gameManager.PlayerInputSources.Contains(inputString) == false)
                            JoinPlayer(inputString);
                    }
                }
                if (buttonsPressed.Contains(inputManager.ControllerArray[i].startButton) && canStartGame == true)
                    ProceedToGameIntro();
            }
        }
    }

    private void JoinPlayer(string inputSourceName)
    {
        playerJoined[currentPlayerIndex] = true;

        gameManager.PlayerInputSources[currentPlayerIndex] = inputSourceName;

        HideJoinPrompt(currentPlayerIndex);
        currentPlayerIndex++;

        if (currentPlayerIndex >= 2) //Allow the game to start
            AllowGameStart();
    }

    private void HideJoinPrompt(int index)
    {
        //Hide the prompts, disable the animated prompt and hide it. Show the "OK!" message.
        animatedPrompts[index].GetComponent<Animator>().enabled = false;
        animatedPrompts[index].GetComponent<SpriteRenderer>().enabled = false;

        okPrompts[index].GetComponent<SpriteRenderer>().enabled = true;
    }

    private void AllowGameStart()
    {
        //Show the Start indicator and allow the game to start.
        startText.GetComponent<SpriteRenderer>().color = startActiveColour;
        startPrompt.GetComponent<SpriteRenderer>().enabled = true;

        canStartGame = true;
    }

    private void ProceedToGameIntro()
    {
        inputManager.Key_Pressed -= ProcessKeys;
        inputManager.Button_Pressed -= ProcessButtons;

        Destroy(gameObject);

        gameManager.CurrentGameState = GameManager.GameState.Intro;
        gameManager.StartIntro();
    }
}
