using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseMenuScript : MonoBehaviour 
{
    private const float THUMBSTICK_DEADZONE = 0.1f;

    private GameManager gameManager = null;
    private InputManager inputManager = null;

    private GameObject[] menuOptions = new GameObject[4];

    private GameObject cameraObj = null;
    private GameObject cursor = null;

    private Color hide = new Color(0, 0, 0, 0);

    private int currentSelectionID = 0;
    private int menuOwnerInputID = -1;

    private bool hasMovedThumbstick = false;
    private bool onControlsScreen = false;

    public int MenuOwnerNum
    {
        get { return menuOwnerInputID; }
        set { menuOwnerInputID = value; }
    }

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

        inputManager = InputManager.Instance;

        gameManager.GameRestart += OnGameReset;

        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        cursor = gameObject.transform.FindChild("Cursor").gameObject;

        menuOptions[0] = gameObject.transform.FindChild("Text_Resume").gameObject; ;
        menuOptions[1] = gameObject.transform.FindChild("Text_Restart").gameObject;
        menuOptions[2] = gameObject.transform.FindChild("Text_Controls").gameObject;
        menuOptions[3] = gameObject.transform.FindChild("Text_Exit").gameObject; ;

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
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            if (onControlsScreen)
                ToggleControlsScreenDisplay(false);
        }
        else if (showMenu == true)//Show the menu
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }    
    }

    private void ProcessButtons(List<string> buttonsPressed)
    {
        if (gameManager.IsGamePaused == true && menuOwnerInputID != -1)
        {
            if (buttonsPressed.Contains(inputManager.ControllerArray[menuOwnerInputID].buttonA) )
            {
                HandleSelection(currentSelectionID);
            }
        }
    }

    private void ProcessThumbstick(int controllerNum, Vector2 axisValues)
    {
        if ((controllerNum - 1) == menuOwnerInputID)
        {
            if (!onControlsScreen)
            {
                if (axisValues.y < -THUMBSTICK_DEADZONE && hasMovedThumbstick == false) //Move down
                {
                    hasMovedThumbstick = true;
                    currentSelectionID++;

                    if (currentSelectionID >= menuOptions.Length)
                        currentSelectionID = 0;

                    PositionCursor(currentSelectionID);
                }
                else if (axisValues.y > THUMBSTICK_DEADZONE && hasMovedThumbstick == false) //Move up
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
    }

    private void ProcessPressedKeys(List<string> keysPressed)
    {
        if (gameManager.IsGamePaused == true && menuOwnerInputID != -1)
        {
            if (!onControlsScreen)
            {
                if (keysPressed.Contains(inputManager.KeybindArray[0].downKey.ToString()) || keysPressed.Contains(inputManager.KeybindArray[1].downKey.ToString()))
                {
                    currentSelectionID++;

                    if (currentSelectionID >= menuOptions.Length)
                        currentSelectionID = 0;

                    PositionCursor(currentSelectionID);
                }
                else if (keysPressed.Contains(inputManager.KeybindArray[0].upKey.ToString()) || keysPressed.Contains(inputManager.KeybindArray[1].upKey.ToString()))
                {
                    currentSelectionID--;

                    if (currentSelectionID < 0)
                        currentSelectionID = menuOptions.Length - 1;

                    PositionCursor(currentSelectionID);
                }
            }

            if (keysPressed.Contains(inputManager.KeybindArray[0].buttonAKey.ToString()) || keysPressed.Contains(inputManager.KeybindArray[1].buttonAKey.ToString()))
                HandleSelection(currentSelectionID);
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
                gameManager.ShowHidePauseMenu(menuOwnerInputID, false);
                break;
            case 1: //Restart the game
                gameManager.RestartGame();
                break;
            case 2: //Show the Controls screen
                if (!onControlsScreen)
                    ToggleControlsScreenDisplay(true);
                else
                    ToggleControlsScreenDisplay(false);
                break;
            case 3: //Quit the game
                Application.Quit();
                break;
        }
    }

    private void OnGameReset()
    {
        if (gameManager.PlayerInputSources[0].Contains("Keybinds") || gameManager.PlayerInputSources[1].Contains("Keybinds")) //Subscribe to all keyboard-related events
        {
            inputManager.Key_Pressed -= ProcessPressedKeys;
        }
        if (gameManager.PlayerInputSources[0].Contains("GamepadControls") || gameManager.PlayerInputSources[1].Contains("GamepadControls")) //Subscribe to all controller-related events
        {
            inputManager.Button_Pressed -= ProcessButtons;
            inputManager.Left_Thumbstick_Axis -= ProcessThumbstick;
        }
    }

    public void EstablishInput()
    {
        if (gameManager.PlayerInputSources[0].Contains("Keybinds") || gameManager.PlayerInputSources[1].Contains("Keybinds") ) //Subscribe to all keyboard-related events
        {
            inputManager.Key_Pressed += ProcessPressedKeys;
        }
        if (gameManager.PlayerInputSources[0].Contains("GamepadControls") || gameManager.PlayerInputSources[1].Contains("GamepadControls") ) //Subscribe to all controller-related events
        {
            inputManager.Button_Pressed += ProcessButtons;
            inputManager.Left_Thumbstick_Axis += ProcessThumbstick;
        }
    }

    private void ToggleControlsScreenDisplay(bool showScreen) //Whether or not to display the Controls screen
    {
        GameObject controlsScreenObj = null;
        if (showScreen)
        {
            onControlsScreen = true;
            controlsScreenObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/New Prefabs/Controls Menu"));
            controlsScreenObj.name = "Controls Menu";
            controlsScreenObj.transform.SetParent(cameraObj.transform);
            controlsScreenObj.transform.localPosition = Vector2.zero;
        }
        else
        {
            controlsScreenObj = cameraObj.transform.FindChild("Controls Menu").gameObject;

            if (controlsScreenObj != null)
                Destroy(controlsScreenObj);

            onControlsScreen = false;
        }
    }
}
