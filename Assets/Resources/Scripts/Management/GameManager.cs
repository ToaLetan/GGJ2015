using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public const float ROUND_LENGTH = 150; //Rounds are 2 minutes 30 seconds. //150
    public const float NUM_ROUNDS = 3; //Play up to 3 rounds max.

    private const float ICON_OFFSET = 22.0f;
    private const float INTRO_TIME = 1.0f;
    private const float SLIDER_MOVE_SPEED = 15.0f;

    private const int NUM_OF_PLAYERS = 2; //This is a 2-player game.

    public enum GameState { JoinScreen, Intro, InGame, PostGame };

    public delegate void GameEvent();

    public GameEvent GameRestart;

    public List<string> PlayerInputSources = new List<string>(); //Where players are getting their controls from (controller 1-2 or keyboard 1-2) by name.

    private Timer roundIntroTimer = null;
    private Timer gameTimer = null;

    private GameObject[] playerWinsText = new GameObject[2];
    private GameObject[] playerArray = new GameObject[2];

    [SerializeField]
    private GameObject sliderUI = null;
    [SerializeField]
    private GameObject pauseMenu = null;
    [SerializeField]
    private GameObject player1SlicesCounter = null;
    [SerializeField]
    private GameObject player2SlicesCounter = null;

    private SpriteRenderer[] slicesOnesRenderers = new SpriteRenderer[2];
    private SpriteRenderer[] slicesTensRenderers = new SpriteRenderer[2];
    private SpriteRenderer[] slicesHundredsRenderers = new SpriteRenderer[2];

    private GameState currentGameState = GameState.JoinScreen;

    private Vector3 sliderDestination = Vector3.zero;
    private Vector3 sliderStartPos = Vector3.zero;

    private int[] playerScores = new int[2];
    private int[] playerWins = new int[2];

    private float sliderAcceleration = 0.0f;

    private int currentRound = 0;
    private int introPhase = 0; //0 = READY?, 1 = EAT!

    private bool isGamePaused = false;
    private bool isSliderAtDestination = true;
    private bool isGameOver = false;
    private bool isPlayingIntro = false;

    private PizzaManager pizzaManager = null;

    private InputManager inputManager = null;

    public InputManager GameInputManager
    {
        get { return inputManager; }
    }

    public GameState CurrentGameState
    {
        get { return currentGameState; }
        set { currentGameState = value; }
    }

    public Timer GameTimer
    {
        get { return gameTimer; }
    }

    public bool IsGamePaused
    {
        get { return isGamePaused; }
        set { isGamePaused = value; }
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }

    public bool IsPlayingIntro
    {
        get { return isPlayingIntro; }
        set { isPlayingIntro = false; }
    }

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;

        for (int i = 0; i < NUM_OF_PLAYERS; i++)
        {
            playerScores[i] = 0;
            playerWins[i] = 0;

            PlayerInputSources.Add("");

            playerWinsText[i] = GameObject.Find("Text_Wins_P" + (i+1) );
            playerArray[i] = GameObject.Find("Player_" + (i+1) );
        }

        if (sliderUI != null)
            sliderStartPos = sliderUI.transform.position;

        if (player1SlicesCounter != null && player2SlicesCounter != null)
        {
            slicesOnesRenderers[0] = player1SlicesCounter.transform.FindChild("Slices_Ones").GetComponent<SpriteRenderer>();
            slicesTensRenderers[0] = player1SlicesCounter.transform.FindChild("Slices_Tens").GetComponent<SpriteRenderer>();
            slicesHundredsRenderers[0] = player1SlicesCounter.transform.FindChild("Slices_Hundreds").GetComponent<SpriteRenderer>();

            slicesOnesRenderers[1] = player2SlicesCounter.transform.FindChild("Slices_Ones").GetComponent<SpriteRenderer>();
            slicesTensRenderers[1] = player2SlicesCounter.transform.FindChild("Slices_Tens").GetComponent<SpriteRenderer>();
            slicesHundredsRenderers[1] = player2SlicesCounter.transform.FindChild("Slices_Hundreds").GetComponent<SpriteRenderer>();
        }

        currentRound = 1; //Obviously we start at the first round.

        gameTimer = new Timer(ROUND_LENGTH);
        gameTimer.OnTimerComplete += OnRoundOver;

        isGamePaused = true;
        isPlayingIntro = true;

        pizzaManager = gameObject.GetComponent<PizzaManager>(); //Pizza Manager is attached to this.
	}
	
	// Update is called once per frame
	void Update () 
    {
        inputManager.Update();

        if (isGamePaused == false && currentGameState == GameState.InGame)
        {
            gameTimer.Update();
        }

        if (currentGameState == GameState.Intro || currentGameState == GameState.InGame)
        {
            if (isSliderAtDestination == false)
                UpdateSliderMovement();
            else
                roundIntroTimer.Update();
        }
	}

    public void StartIntro() //Called after the players have joined.
    {
        roundIntroTimer = new Timer(INTRO_TIME);
        isSliderAtDestination = false;
        ResetIntro();

        //Establish the player inputs from the join menu.
        for (int i = 0; i < playerArray.Length; i++)
        {
            playerArray[i].GetComponent<PlayerScript>().AssignInput();
        }

        //Establish Pause Menu input.
        pauseMenu.GetComponent<PauseMenuScript>().EstablishInput();
    }

    public void AdjustScore(int playerNum, int scoreToAdd)
    {
        playerScores[playerNum - 1] += scoreToAdd;
        UpdateSlicesCounter(playerNum);
    }

    private void UpdateSlicesCounter(int playerNum)
    {
        //Get the ones, tens, and hundreds values
        int hundredsValue = Mathf.FloorToInt(playerScores[playerNum - 1] / 100);
        int tensValue = Mathf.FloorToInt(playerScores[playerNum - 1] / 10);
        int onesValue = Mathf.FloorToInt(playerScores[playerNum - 1] % 10);

        slicesHundredsRenderers[playerNum - 1].sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_Text", "Num_" + hundredsValue);
        slicesTensRenderers[playerNum - 1].sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_Text", "Num_" + tensValue);
        slicesOnesRenderers[playerNum - 1].sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_Text", "Num_" + onesValue);
    }

    private void OnRoundOver()
    {
        //Determine the winner here.
        if (playerScores[0] > playerScores[1]) //Player 1 wins
        {
            playerWins[0]++;
            DisplayWin(1);
        }
        else if (playerScores[0] < playerScores[1]) //Player 2 wins
        {
            playerWins[1]++;
            DisplayWin(2);
        }
        else if (playerScores[0] == playerScores[1]) //It's a tie, everyone wins
        {
            playerWins[0]++;
            playerWins[1]++;

            DisplayWin(1);
            DisplayWin(2);
        }

        //Reset round score
        for (int i = 0; i < NUM_OF_PLAYERS; i++)
        {
            playerScores[i] = 0;
            UpdateSlicesCounter(i+1);
        }

        //Prepare the next round if it's not over.
        if (currentRound < NUM_ROUNDS)
        {
            //Increment the round number, reset timers, reset the necessary things for playing the intro and run the intro again.
            currentRound++;

            gameTimer.ResetTimer();
            GameTimer gameTimeDisplay = GameObject.Find("Timer").GetComponent<GameTimer>();
            gameTimeDisplay.UpdateDisplay(true);

            ResetIntro();

            isGamePaused = true;

            pizzaManager.ResetPizzaSpawns();

            ResetPlayers();
        }
        else //The game is over, play the ending.
        {
            isGameOver = true;
            DisplayWinner();
        }

    }

    private void DisplayWin(int playerNumber)
    {
        int playerID = playerNumber - 1;

        Vector3 iconPos = playerWinsText[playerID].transform.position;
        iconPos.x += ICON_OFFSET + (ICON_OFFSET * playerWins[playerID]);
        GameObject winIcon = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Icon_Pizza"), iconPos, Quaternion.identity) as GameObject;
        winIcon.transform.parent = playerWinsText[playerID].transform;
    }

    private void StartRound()
    {
        isPlayingIntro = false;
        gameTimer.StartTimer();
        isGamePaused = false;
    }

    private void PlayRoundIntro()
    {
        GameObject sliderText = sliderUI.transform.FindChild("Slider_Text").gameObject;

        if (introPhase < 2)
        {
            switch (introPhase)
            {
                case 0:
                    sliderText.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Ready");
                    break;
                case 1:
                    sliderText.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Eat");
                    break;
                default:
                    break;
            }

            introPhase++;
            roundIntroTimer.ResetTimer(true);
        }
        else //START THE GAME ALREADY after we move back to the left
        {
            roundIntroTimer.OnTimerComplete -= PlayRoundIntro;

            sliderDestination = sliderStartPos;
            isSliderAtDestination = false;

            currentGameState = GameState.InGame;
        } 
    }

    private void UpdateSliderMovement()
    {
        if (sliderDestination != sliderStartPos)
        {
            if (sliderUI.transform.position.x < sliderDestination.x) //Move right
            {
                Vector3 newPos = sliderUI.transform.position;
                sliderAcceleration += 1.0f;
                newPos.x += sliderAcceleration * SLIDER_MOVE_SPEED * Time.deltaTime;

                sliderUI.transform.position = newPos;
            }
            else
            {
                isSliderAtDestination = true;
                sliderAcceleration = 0.0f;
            }
        }
        else
        {
            if (sliderUI.transform.position.x > sliderDestination.x) //Move left
            {
                Vector3 newPos = sliderUI.transform.position;
                sliderAcceleration += 1.0f;
                newPos.x += sliderAcceleration * SLIDER_MOVE_SPEED * Time.deltaTime * -1;

                sliderUI.transform.position = newPos;
            }
            else
            {
                isSliderAtDestination = true;
                sliderAcceleration = 0.0f;

                StartRound();
            }   
        }
    }

    private void ResetIntro()
    {
        roundIntroTimer.ResetTimer(true);

        roundIntroTimer.OnTimerComplete += PlayRoundIntro;

        sliderDestination = new Vector3(-74, 92, 0);
        isSliderAtDestination = false;
        introPhase = 0;

        GameObject sliderText = sliderUI.transform.FindChild("Slider_Text").gameObject;
        sliderText.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Ready");
    }

    private void ResetPlayers() //Clear any held slices on players, reset prefabs
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            Vector2 playerPosition = playerArray[i].transform.position;

            //Destroy(playerArray[i]);
            playerArray[i].GetComponent<PlayerScript>().DestroyPlayer();

            //Reinstantiate the player prefabs and assign their input
            playerArray[i] = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player_" + (i+1) ), playerPosition, Quaternion.identity);

            PlayerScript newPlayerScript = playerArray[i].GetComponent<PlayerScript>();
            newPlayerScript.InitPlayer();
            newPlayerScript.AssignInput();
        }
    }

    private void DisplayWinner()
    {
        //Reset the slider so it moves.
        sliderDestination = new Vector3(74, 92, 0);
        isSliderAtDestination = false;

        //Change the slider text to say "Player", move it over to leave some room.
        GameObject sliderText = sliderUI.transform.FindChild("Slider_Text").gameObject;
        sliderText.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Player");
        Vector3 initialSliderTextPos = sliderText.transform.position; //Saving this in case of a tie, don't have to move the text.
        sliderText.transform.position += new Vector3(-ICON_OFFSET * 2, 0, 0);

        //Prepare the position for the player num.
        Vector3 winnerNumPos = sliderText.transform.position;
        winnerNumPos.x += ICON_OFFSET * 1.75f;

        //Prepare the display for the winner's number.
        GameObject winnerNumText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Winner_Num"), winnerNumPos, Quaternion.identity) as GameObject;
        winnerNumText.transform.parent = sliderUI.transform;

        //Prepare the position for the "wins!" text.
        Vector3 winGameText = winnerNumText.transform.position;
        winGameText.x += ICON_OFFSET * 1.75f;

        //Create the "wins!" text.
        GameObject winsText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Text_Winner"), winGameText, Quaternion.identity) as GameObject;
        winsText.transform.parent = sliderUI.transform;

        if (playerWins[0] > playerWins[1] ) //Player 1 won the game
        {
            winnerNumText.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Number_1");
        }
        else if (playerWins[0] < playerWins[1]) //Player 2 won the game
        {
            winnerNumText.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Number_2");
        }
        else if (playerWins[0] == playerWins[1]) //Holy shit it's a tie!!!
        {
            sliderText.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Tied");
            sliderText.transform.position = initialSliderTextPos; //Move the text back, we don't need a lot of room.

            //Destroy the stuff we don't need.
            GameObject.Destroy(winnerNumText);
            GameObject.Destroy(winsText);
        }
    }

    public void ShowHidePauseMenu(int playerInputID, bool showMenu)
    {
        if (showMenu == true) //The menu must be shown
        {
            isGamePaused = true;

            pauseMenu.GetComponent<PauseMenuScript>().TogglePauseMenu(true);
            pauseMenu.GetComponent<PauseMenuScript>().MenuOwnerNum = playerInputID;
        }
        else //Close the menu
        {
            if(isGameOver == false)
                isGamePaused = false;

            pauseMenu.GetComponent<PauseMenuScript>().TogglePauseMenu(false);
            pauseMenu.GetComponent<PauseMenuScript>().MenuOwnerNum = -1;
        }
    }

    public void RestartGame()
    {
        if (GameRestart != null) //Fire an event to everything that needs to unsub from events.
            GameRestart();

        SceneManager.LoadScene(0);
    }
}
