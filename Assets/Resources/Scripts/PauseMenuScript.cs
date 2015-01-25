using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseMenuScript : MonoBehaviour 
{
    private const float THUMBSTICK_DEADZONE = 0.1f;

    private GameManager gameManager = null;
    private InputManager inputManager = null;

    private GameObject[] menuOptions = new GameObject[3];

    private GameObject cursor = null;

    private Color hide = new Color(0, 0, 0, 0);

    private int currentSelectionID = 0;
    public int menuOwnerNum = 0;

    private bool hasMovedThumbstick = false;

    public int MenuOwnerNum
    {
        get { return menuOwnerNum; }
        set { menuOwnerNum = value; }
    }

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

        inputManager = InputManager.Instance;

        inputManager.Button_Pressed += ProcessButtons;
        inputManager.Left_Thumbstick_Axis += ProcessThumbstick;

        gameManager.GameRestart += OnGameReset;

        cursor = gameObject.transform.FindChild("PauseMenu_Cursor").gameObject;

        menuOptions[0] = gameObject.transform.FindChild("Text_Resume").gameObject; ;
        menuOptions[1] = gameObject.transform.FindChild("Text_Restart").gameObject; ;
        menuOptions[2] = gameObject.transform.FindChild("Text_Exit").gameObject; ;

        PositionCursor(currentSelectionID);

        TogglePauseMenu(false);
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void TogglePauseMenu(bool showMenu)
    {
        if (showMenu == false) //Hide the menu
        {
            gameObject.GetComponent<SpriteRenderer>().color = hide;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = hide;
                }
            }
        }
        else if (showMenu == true)//Show the menu
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }    
    }

    private void ProcessButtons(List<string> buttonsPressed)
    {
        if (gameManager.IsGamePaused == true && menuOwnerNum != 0)
        {
            if (buttonsPressed.Contains(inputManager.ControllerArray[menuOwnerNum - 1].buttonA) )
            {
                HandleSelection(currentSelectionID);
            }
        }
    }

    private void ProcessThumbstick(int controllerNum, Vector2 axisValues)
    {
        if (controllerNum == menuOwnerNum)
        {
            if (axisValues.y < -THUMBSTICK_DEADZONE && hasMovedThumbstick == false)
            {
                hasMovedThumbstick = true;
                currentSelectionID++;

                if (currentSelectionID >= menuOptions.Length)
                    currentSelectionID = 0;

                PositionCursor(currentSelectionID);
            }
            else if (axisValues.y > THUMBSTICK_DEADZONE && hasMovedThumbstick == false)
            {
                hasMovedThumbstick = true;
                currentSelectionID--;

                if (currentSelectionID < 0)
                    currentSelectionID = menuOptions.Length - 1;

                PositionCursor(currentSelectionID);
            }
            else if (axisValues.y >= -THUMBSTICK_DEADZONE && axisValues.y <= THUMBSTICK_DEADZONE && hasMovedThumbstick == true) //reset the ability to flick to the next option.
            {
                hasMovedThumbstick = false;
            }
        }
    }

    private void PositionCursor(int cursorIndex)
    {
        Vector3 newCursorPos = cursor.transform.position;

        newCursorPos.y = menuOptions[cursorIndex].transform.position.y;

        cursor.transform.position = newCursorPos;
    }

    private void HandleSelection(int cursorIndex)
    {
        switch(cursorIndex)
        {
            case 0: //Resume the game
                gameManager.ShowHidePauseMenu(menuOwnerNum, false);
                break;
            case 1: //Restart the game
                gameManager.RestartGame();
                break;
            case 2: //Quit the game
                Application.Quit();
                break;
        }
    }

    private void OnGameReset()
    {
        inputManager.Button_Pressed -= ProcessButtons;
        inputManager.Left_Thumbstick_Axis -= ProcessThumbstick;
    }
}
