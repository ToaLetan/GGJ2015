using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour 
{
    private const float MOVE_SPEED = 1.5f;
    private const float ROTATE_SPEED = 1.0f;

    public int PlayerNum = 0;

    public enum InputSource { Controller, Keyboard };

    private InputManager inputManager = null;
    private GameManager gameManager = null;

    private GameObject[] buttonPrompts = new GameObject[4];
    private SpriteRenderer[] buttonPromptRenderers = new SpriteRenderer[4];

    [SerializeField]
    private GameObject triggerPrompt = null;

    private SpriteRenderer triggerPromptRenderer = null;
    private SpriteRenderer triggerPromptTextRenderer = null;

    private GameObject activeTentacle = null;
    private GameObject tentacleGrabber = null;
    private Rigidbody2D tentacleGrabberRigidbody = null;

    private InputSource currentInputSource;

    private int playerInputID = 0;

    public GameObject ActiveTentacle
    {
        get { return activeTentacle; }
    }

    public GameObject TentacleGrabber
    {
        get { return tentacleGrabber; }
    }

    public int PlayerInputID
    {
        get { return playerInputID; }
        set { playerInputID = value; }
    }

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

        gameManager.GameRestart += OnGameReset;

        //Get the button prompts attached to each tentacle's first link.
        buttonPrompts[0] = gameObject.transform.FindChild("Tentacle_A").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
        buttonPrompts[1] = gameObject.transform.FindChild("Tentacle_B").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
        buttonPrompts[2] = gameObject.transform.FindChild("Tentacle_X").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
        buttonPrompts[3] = gameObject.transform.FindChild("Tentacle_Y").FindChild("Tentacle_Link_0").GetChild(0).gameObject;

        for (int i = 0; i < buttonPrompts.Length; i++)
        {
            buttonPromptRenderers[i] = buttonPrompts[i].GetComponent<SpriteRenderer>();
        }


        if(triggerPrompt != null)
        {
            triggerPromptRenderer = triggerPrompt.transform.GetComponent<SpriteRenderer>();
            triggerPromptTextRenderer = triggerPrompt.transform.GetChild(0).GetComponent<SpriteRenderer>();
            triggerPromptRenderer.enabled = false;
            triggerPromptTextRenderer.enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () 
    {

	}

    private void ProcessThumbstickMovement(int controllerNum, Vector2 thumbstickPosition)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            if ((controllerNum - 1) == playerInputID && activeTentacle != null)
            {
                if (thumbstickPosition != Vector2.zero)
                {
                    tentacleGrabber.transform.position += new Vector3(thumbstickPosition.x, thumbstickPosition.y, 0) * MOVE_SPEED * Time.deltaTime;
                }
                else
                    tentacleGrabberRigidbody.angularVelocity = 0;
            }
        }
    }

    private void ProcessButtonPresses(List<string> buttonsPressed)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            if (buttonsPressed.Contains(inputManager.ControllerArray[playerInputID].buttonA))
            {
                SelectTentacle('A');
            }
            if (buttonsPressed.Contains(inputManager.ControllerArray[playerInputID].buttonB))
            {
                SelectTentacle('B');
            }
            if (buttonsPressed.Contains(inputManager.ControllerArray[playerInputID].buttonX))
            {
                SelectTentacle('X');
            }
            if (buttonsPressed.Contains(inputManager.ControllerArray[playerInputID].buttonY))
            {
                SelectTentacle('Y');
            }
        }
        if (buttonsPressed.Contains(inputManager.ControllerArray[playerInputID].startButton)) //Show/hide the pause menu.
        {
            if(gameManager.IsGamePaused == false || gameManager.IsGameOver == true)
                gameManager.ShowHidePauseMenu(playerInputID, true);
            else if(gameManager.IsGamePaused == true && gameManager.IsPlayingIntro == false)
                gameManager.ShowHidePauseMenu(playerInputID, false);
        }
    }

    private void ProcessTrigger(int controllerNum, float triggerValue)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            if ((controllerNum - 1) == playerInputID && triggerValue < 0.0f)
            {
                if (activeTentacle != null)
                {
                    Grab();

                    //Highlight RT
                    //triggerPromptRenderer.sprite = Resources.Load<Sprite>("Sprites/UI/Active_Trigger_Right");
                    triggerPromptRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_ButtonPrompts", "Active_Trigger_Right");
                }
            }
            if (controllerNum == PlayerNum && triggerValue >= 0.0f) //Let go of the platter if previously holding it.
            {
                if (activeTentacle != null)
                {
                    if (tentacleGrabber.GetComponent<GrabScript>().IsHoldingPlatter == true)
                        tentacleGrabber.GetComponent<GrabScript>().IsHoldingPlatter = false;

                    //Highlight RT
                    //triggerPromptRenderer.sprite = Resources.Load<Sprite>("Sprites/UI/Trigger_Right");
                    triggerPromptRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_ButtonPrompts", "Trigger_Right");
                }
            }
        }
    }

    private void ProcessHeldKeys(List<string> keysHeld)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            if (activeTentacle != null)
            {
                //If any directional keys are being held, determine a direction based on what's being held similarly to how thumbsticks go from -1 to 1.
                if (keysHeld.Contains(inputManager.KeybindArray[playerInputID].upKey.ToString()) || keysHeld.Contains(inputManager.KeybindArray[playerInputID].downKey.ToString())
                    || keysHeld.Contains(inputManager.KeybindArray[playerInputID].leftKey.ToString()) || keysHeld.Contains(inputManager.KeybindArray[playerInputID].rightKey.ToString()))
                {
                    int directionX = 0;
                    int directionY = 0;

                    if (keysHeld.Contains(inputManager.KeybindArray[playerInputID].upKey.ToString()))
                        directionY += 1;
                    if (keysHeld.Contains(inputManager.KeybindArray[playerInputID].downKey.ToString()))
                        directionY += -1;
                    if (keysHeld.Contains(inputManager.KeybindArray[playerInputID].leftKey.ToString()))
                        directionX += -1;
                    if (keysHeld.Contains(inputManager.KeybindArray[playerInputID].rightKey.ToString()))
                        directionX += 1;

                    //tentacleGrabber.transform.localPosition += new Vector3(directionX, directionY, 0) * MOVE_SPEED * Time.deltaTime;
                    tentacleGrabberRigidbody.AddForce(new Vector2(directionX, directionY) * MOVE_SPEED * Time.deltaTime);
                }
                else
                    tentacleGrabberRigidbody.angularVelocity = 0;

                if(keysHeld.Contains(inputManager.KeybindArray[playerInputID].rightTriggerKey.ToString()) )
                {
                    if (activeTentacle != null)
                    {
                        Grab();

                        //Highlight prompt
                        //triggerPromptRenderer.sprite = Resources.Load<Sprite>("Sprites/UI/Active_Key_RT" + (playerInputID+ 1) );
                        triggerPromptRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_ButtonPrompts", "Active_Key_RT" + (playerInputID + 1));
                    }
                }
            }
        }
    }

    private void ProcessPressedKeys(List<string> keysPressed)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            if (keysPressed.Contains(inputManager.KeybindArray[playerInputID].buttonAKey.ToString() ))
            {
                SelectTentacle('A');
            }
            if (keysPressed.Contains(inputManager.KeybindArray[playerInputID].buttonBKey.ToString() ))
            {
                SelectTentacle('B');
            }
            if (keysPressed.Contains(inputManager.KeybindArray[playerInputID].buttonXKey.ToString() ))
            {
                SelectTentacle('X');
            }
            if (keysPressed.Contains(inputManager.KeybindArray[playerInputID].buttonYKey.ToString() ))
            {
                SelectTentacle('Y');
            }
        }
        if (keysPressed.Contains(inputManager.KeybindArray[playerInputID].startKey.ToString() )) //Show/hide the pause menu.
        {
            if (gameManager.IsGamePaused == false || gameManager.IsGameOver == true)
                gameManager.ShowHidePauseMenu(playerInputID, true);
            else if (gameManager.IsGamePaused == true && gameManager.IsPlayingIntro == false)
                gameManager.ShowHidePauseMenu(playerInputID, false);
        }
    }

    private void ProcessReleasedKeys(List<string> keysReleased)
    {
        if (keysReleased.Contains(inputManager.KeybindArray[playerInputID].rightTriggerKey.ToString())) //Let go of the held object
        {
            if (activeTentacle != null)
            {
                if (tentacleGrabber.GetComponent<GrabScript>().IsHoldingPlatter == true)
                    tentacleGrabber.GetComponent<GrabScript>().IsHoldingPlatter = false;

                //Highlight RT
                // triggerPromptRenderer.sprite = Resources.Load<Sprite>("Sprites/UI/Key_RT" + (playerInputID + 1));
                triggerPromptRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_ButtonPrompts", "Key_RT" + (playerInputID + 1));
            }
        }
    }

    private void SelectTentacle(char tentacleButton)
    {
        if (activeTentacle != null) //If there's already a tentacle in use, de-highlight it.
        {
            tentacleGrabber.GetComponent<GrabScript>().OnGrabEnter -= HighlightTriggerPrompt;
        }

        //Get the active tentacle, the end of the tentacle, and set its script to be active.
        activeTentacle = gameObject.transform.FindChild("Tentacle_" + tentacleButton).gameObject;
        tentacleGrabber = activeTentacle.transform.FindChild("Tentacle_Grabber").gameObject;
        tentacleGrabberRigidbody = tentacleGrabber.GetComponent<Rigidbody2D>();

        tentacleGrabber.GetComponent<GrabScript>().IsTentacleActive = true;
        tentacleGrabber.GetComponent<GrabScript>().OnGrabEnter += HighlightTriggerPrompt;

        //Highlight the selected button prompt.
        HighlightButtonPrompt(tentacleButton);
    }

    private void Grab()
    {
        if (tentacleGrabber.GetComponent<GrabScript>().IsHoldingPizza == false && tentacleGrabber.GetComponent<GrabScript>().IsHoldingPlatter == false
                        && tentacleGrabber.GetComponent<GrabScript>().CollidingSlice != null)
        {
            tentacleGrabber.GetComponent<GrabScript>().IsHoldingPizza = true;

            if (tentacleGrabber.GetComponent<GrabScript>().CollidingSlice.tag == "Pizza") //Grab the slice
            {
                //Disable physics on the slice.
                GameObject.Destroy(tentacleGrabber.GetComponent<GrabScript>().CollidingSlice.GetComponent<Rigidbody2D>());
                //tentacleGrabber.GetComponent<GrabScript>().CollidingSlice.GetComponent<EdgeCollider2D>().isTrigger = true;

                tentacleGrabber.GetComponent<GrabScript>().CollidingSlice.transform.parent = tentacleGrabber.transform;
            }
        }
        else if (tentacleGrabber.GetComponent<GrabScript>().IsHoldingPizza == false && tentacleGrabber.GetComponent<GrabScript>().CollidingPlatter != null)
        {
            if (tentacleGrabber.GetComponent<GrabScript>().CollidingPlatter.tag == "Platter") //Spin the platter
            {
                tentacleGrabber.GetComponent<GrabScript>().IsHoldingPlatter = true;

                Quaternion platterRotation = tentacleGrabber.GetComponent<GrabScript>().CollidingPlatter.transform.localRotation;

                platterRotation.eulerAngles += new Vector3(0, 0, tentacleGrabber.transform.localRotation.eulerAngles.z - 180) * ROTATE_SPEED * Time.deltaTime;

                tentacleGrabber.GetComponent<GrabScript>().CollidingPlatter.transform.localRotation = platterRotation;
            }
        }
    }

    private void HighlightButtonPrompt(char button)
    {
        int currentActiveButtonIndex = 0;

        switch(button)
        {
            case 'A':
                currentActiveButtonIndex = 0;
                break;
            case 'B':
                currentActiveButtonIndex = 1;
                break;
            case 'X':
                currentActiveButtonIndex = 2;
                break;
            case 'Y':
                currentActiveButtonIndex = 3;
                break;
            default:
                break;
        }

        if(currentInputSource == InputSource.Controller)
            buttonPromptRenderers[currentActiveButtonIndex].sprite = Resources.Load<Sprite>("Sprites/UI/Active_Button_" + button);
        else if (currentInputSource == InputSource.Keyboard)
            buttonPromptRenderers[currentActiveButtonIndex].sprite = Resources.Load<Sprite>("Sprites/UI/Active_Key_" + button + (playerInputID + 1));
            

        //Loop through the other buttons and de-highlight any that were previously highlighted
        for (int i = 0; i < buttonPrompts.Length; i++)
        {
            char currentButton = ' ';

            if (i != currentActiveButtonIndex)
            {
                switch(i)
                {
                    case 0:
                        currentButton = 'A';
                        break;
                    case 1:
                        currentButton = 'B';
                        break;
                    case 2:
                        currentButton = 'X';
                        break;
                    case 3:
                        currentButton = 'Y';
                        break;
                    default:
                        break;
                }

                if (currentButton != ' ')
                {
                    if(currentInputSource == InputSource.Controller)
                        buttonPromptRenderers[i].sprite = Resources.Load<Sprite>("Sprites/UI/Button_" + currentButton);
                    else if (currentInputSource == InputSource.Keyboard)
                        buttonPromptRenderers[i].sprite = Resources.Load<Sprite>("Sprites/UI/Key_" + currentButton + (playerInputID + 1));
                }
            }
        }
    }

    private void HighlightTriggerPrompt()
    {
        if (activeTentacle != null)
        {
            if (tentacleGrabber.GetComponent<GrabScript>().CollidingSlice != null) //Show "GRAB"
            {
                triggerPromptTextRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_Text", "Text_Grab");

                if (triggerPromptRenderer.enabled == false)
                {
                    triggerPromptRenderer.enabled = true;
                    triggerPromptTextRenderer.enabled = true;
                }
            }
            else if (tentacleGrabber.GetComponent<GrabScript>().CollidingPlatter != null) //Show "SPIN"
            {
                triggerPromptTextRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_Text", "Text_Spin");

                if (triggerPromptRenderer.enabled == false)
                {
                    triggerPromptRenderer.enabled = true;
                    triggerPromptTextRenderer.enabled = true;
                }
            }
            else
            {
                triggerPromptRenderer.enabled = false;
                triggerPromptTextRenderer.enabled = false;
            }
        }
    }

    private void OnGameReset()
    {
        if (gameManager.PlayerInputSources[PlayerNum - 1].Contains("Keybinds"))
        {
            currentInputSource = InputSource.Keyboard;

            inputManager.Key_Held -= ProcessHeldKeys;
            inputManager.Key_Pressed -= ProcessPressedKeys;
            inputManager.Key_Released -= ProcessReleasedKeys;
        }
        else if (gameManager.PlayerInputSources[PlayerNum - 1].Contains("GamepadControls"))
        {
            currentInputSource = InputSource.Controller;

            inputManager.Left_Thumbstick_Axis -= ProcessThumbstickMovement;
            inputManager.Button_Pressed -= ProcessButtonPresses;
            inputManager.Right_Trigger_Axis -= ProcessTrigger;
        }
    }

    public void AssignInput()
    {
        playerInputID = int.Parse(gameManager.PlayerInputSources[PlayerNum - 1].Substring(gameManager.PlayerInputSources[PlayerNum - 1].IndexOf(" ") ) );

        if (gameManager.PlayerInputSources[PlayerNum - 1].Contains("Keybinds"))
        {
            currentInputSource = InputSource.Keyboard;

            inputManager.Key_Held += ProcessHeldKeys;
            inputManager.Key_Pressed += ProcessPressedKeys;
            inputManager.Key_Released += ProcessReleasedKeys;

            //Swap all Prompts to display Keys
            //triggerPromptRenderer.sprite = Resources.Load<Sprite>("Sprites/UI/Key_RT" + (playerInputID + 1));
            triggerPromptRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_ButtonPrompts", "Key_RT" + (playerInputID + 1));

            //Loop through the other buttons and de-highlight any that were previously highlighted
            for (int i = 0; i < buttonPrompts.Length; i++)
            {
                char currentButton = ' ';

                    switch(i)
                    {
                        case 0:
                            currentButton = 'A';
                            break;
                        case 1:
                            currentButton = 'B';
                            break;
                        case 2:
                            currentButton = 'X';
                            break;
                        case 3:
                            currentButton = 'Y';
                            break;
                        default:
                            break;
                    }

                    if (currentButton != ' ')
                    {
                        if(currentInputSource == InputSource.Controller)
                            buttonPromptRenderers[i].sprite = Resources.Load<Sprite>("Sprites/UI/Button_" + currentButton);
                        else if (currentInputSource == InputSource.Keyboard)
                            buttonPromptRenderers[i].sprite = Resources.Load<Sprite>("Sprites/UI/Key_" + currentButton + (playerInputID + 1) );
                    }
            }
        }
        else if (gameManager.PlayerInputSources[PlayerNum - 1].Contains("GamepadControls"))
        {
            currentInputSource = InputSource.Controller;

            inputManager.Left_Thumbstick_Axis += ProcessThumbstickMovement;
            inputManager.Button_Pressed += ProcessButtonPresses;
            inputManager.Right_Trigger_Axis += ProcessTrigger;
        }
    }
}
