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

    private List<GameObject> activeTentacles = new List<GameObject>(); //All tentacles that have been selected
    private List<GameObject> activeTentacleGrabbers = new List<GameObject>(); //All endpoints of the selected tentacles
    private List<Rigidbody2D> activeTentacleGrabbersRigidbodies = new List<Rigidbody2D>(); //Rigidbodies of the selected tentacle endpoints
    private List<GrabScript> activeTentaclesScripts = new List<GrabScript>(); //GrabScripts of the selected tentacle endpoints

    private GameObject[] tentacles = new GameObject[4]; //Every tentacle connected to the squid obj

    private InputSource currentInputSource;

    private int playerInputID = 0;

    public List<GameObject> ActiveTentacles
    {
        get { return activeTentacles; }
    }

    public List<GameObject> ActiveTentacleGrabbers
    {
        get { return activeTentacleGrabbers; }
    }

    public int PlayerInputID
    {
        get { return playerInputID; }
        set { playerInputID = value; }
    }

	// Use this for initialization
	void Start () 
    {
        InitPlayer();
    }

    public void InitPlayer()
    {
        if (gameManager == null)
            gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

        if (inputManager == null)
            inputManager = InputManager.Instance;

        tentacles[0] = gameObject.transform.FindChild("Tentacle_A").gameObject;
        tentacles[1] = gameObject.transform.FindChild("Tentacle_B").gameObject;
        tentacles[2] = gameObject.transform.FindChild("Tentacle_X").gameObject;
        tentacles[3] = gameObject.transform.FindChild("Tentacle_Y").gameObject;

        //Get the button prompts attached to each tentacle's first link.
        for (int i = 0; i < buttonPrompts.Length; i++)
        {
            buttonPrompts[i] = tentacles[i].transform.FindChild("Tentacle_Link_0").GetChild(0).gameObject;
            buttonPromptRenderers[i] = buttonPrompts[i].GetComponent<SpriteRenderer>();
        }

        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");

        triggerPrompt = camera.transform.FindChild("HUD Bottom Frame").FindChild("RT_Prompt_Player_" + PlayerNum).gameObject;

        if (triggerPrompt != null)
        {
            triggerPromptRenderer = triggerPrompt.transform.GetComponent<SpriteRenderer>();
            triggerPromptTextRenderer = triggerPrompt.transform.GetChild(0).GetComponent<SpriteRenderer>();
            triggerPromptRenderer.enabled = false;
            triggerPromptTextRenderer.enabled = false;
        }
    }

    public void DestroyPlayer()
    {
        gameManager.GameRestart -= OnGameReset;
        inputManager.Key_Held -= ProcessHeldKeys;
        inputManager.Key_Pressed -= ProcessPressedKeys;
        inputManager.Key_Released -= ProcessReleasedKeys;
        inputManager.Left_Thumbstick_Axis -= ProcessThumbstickMovement;
        inputManager.Button_Pressed -= ProcessButtonPresses;
        inputManager.Right_Trigger_Axis -= ProcessTrigger;

        for (int i = 0; i < tentacles.Length; i++)
            tentacles[i].transform.FindChild("Tentacle_Grabber").GetComponent<GrabScript>().OnGrabEnter -= HighlightTriggerPrompt;

        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () 
    {

	}

    private void ProcessThumbstickMovement(int controllerNum, Vector2 thumbstickPosition)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            if ((controllerNum - 1) == playerInputID && activeTentacles != null)
            {
                if (thumbstickPosition != Vector2.zero)
                {
                    for (int i = 0; i < activeTentacleGrabbers.Count; i++)
                        activeTentacleGrabbersRigidbodies[i].AddForce(new Vector2(thumbstickPosition.x, thumbstickPosition.y) * MOVE_SPEED * Time.deltaTime);
                    //activeTentacleGrabbers[i].transform.position += new Vector3(thumbstickPosition.x, thumbstickPosition.y, 0) * MOVE_SPEED * Time.deltaTime;
                }
                else
                {
                    for (int i = 0; i < activeTentacleGrabbersRigidbodies.Count; i++)
                        activeTentacleGrabbersRigidbodies[i].angularVelocity = 0;
                }
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
                if (activeTentacles.Count > 0)
                {
                    Grab();

                    //Highlight RT
                    //triggerPromptRenderer.sprite = Resources.Load<Sprite>("Sprites/UI/Active_Trigger_Right");
                    triggerPromptRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_ButtonPrompts", "Active_Trigger_Right");
                }
            }
            if (controllerNum == PlayerNum && triggerValue >= 0.0f) //Let go of the platter if previously holding it.
            {
                for (int i = 0; i < activeTentaclesScripts.Count; i++)
                {
                    if (activeTentaclesScripts[i].IsHoldingPlatter == true)
                        activeTentaclesScripts[i].IsHoldingPlatter = false;

                    //Highlight RT
                    //triggerPromptRenderer.sprite = Resources.Load<Sprite>("Sprites/UI/Trigger_Right");
                    
                }
            }
            triggerPromptRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_ButtonPrompts", "Trigger_Right");
        }
    }

    private void ProcessHeldKeys(List<string> keysHeld)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            for (int i = 0; i < activeTentacles.Count; i++)
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
                    activeTentacleGrabbersRigidbodies[i].AddForce(new Vector2(directionX, directionY) * MOVE_SPEED * Time.deltaTime);
                }
                else
                    activeTentacleGrabbersRigidbodies[i].angularVelocity = 0;

                if (keysHeld.Contains(inputManager.KeybindArray[playerInputID].rightTriggerKey.ToString()) )
                {
                    if (activeTentacles.Count > 0)
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
            if (activeTentacles.Count > 0)
            {
                for (int i = 0; i < activeTentaclesScripts.Count; i++)
                {
                    if (activeTentaclesScripts[i].IsHoldingPlatter == true)
                        activeTentaclesScripts[i].IsHoldingPlatter = false;
                }

                //Highlight RT
                // triggerPromptRenderer.sprite = Resources.Load<Sprite>("Sprites/UI/Key_RT" + (playerInputID + 1));
                triggerPromptRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_ButtonPrompts", "Key_RT" + (playerInputID + 1));
            }
        }
    }

    private void SelectTentacle(char tentacleButton)
    {
        GameObject currentSelectedTentacle = gameObject.transform.FindChild("Tentacle_" + tentacleButton).gameObject;

        if (activeTentacles.Contains(currentSelectedTentacle) ) //If there's already a tentacle in use, de-highlight it.
        {
            for (int i = 0; i < activeTentacles.Count; i++)
            {
                if (activeTentacles[i].name == currentSelectedTentacle.name)
                {
                    //Remove the highlight
                    char buttonName = currentSelectedTentacle.name.Substring(currentSelectedTentacle.name.IndexOf('_')+1, 1)[0];

                    ToggleHighlightButtonPrompt(buttonName, false);

                    //Unsub from the OnGrabEnter event and remove the tentacle from the list
                    activeTentaclesScripts[i].OnGrabEnter -= HighlightTriggerPrompt;
                    activeTentacles.Remove(activeTentacles[i]);
                    activeTentacleGrabbers.Remove(activeTentacleGrabbers[i]);
                    activeTentacleGrabbersRigidbodies.Remove(activeTentacleGrabbersRigidbodies[i]);
                    activeTentaclesScripts.Remove(activeTentaclesScripts[i]);
                    break;
                }
            }
        }
        else
        {
            //Add the active tentacle, grabber, grabber rigidbody, and grabber script to the respective lists
            activeTentacles.Add(currentSelectedTentacle);
            GameObject currentGrabber = currentSelectedTentacle.transform.FindChild("Tentacle_Grabber").gameObject;
            GrabScript currentTentacleScript = currentGrabber.GetComponent<GrabScript>();

            activeTentacleGrabbers.Add(currentGrabber);
            activeTentacleGrabbersRigidbodies.Add(currentGrabber.GetComponent<Rigidbody2D>());
            activeTentaclesScripts.Add(currentTentacleScript);

            currentTentacleScript.IsTentacleActive = true;
            currentTentacleScript.OnGrabEnter += HighlightTriggerPrompt;


            //Highlight the selected button prompt.
            ToggleHighlightButtonPrompt(tentacleButton, true);
        }

        //Get the active tentacle, the end of the tentacle, and set its script to be active.
        /*activeTentacles = gameObject.transform.FindChild("Tentacle_" + tentacleButton).gameObject;
        tentacleGrabber = activeTentacles.transform.FindChild("Tentacle_Grabber").gameObject;
        tentacleGrabberRigidbody = tentacleGrabber.GetComponent<Rigidbody2D>();
        activeTentaclesScripts = tentacleGrabber.GetComponent<GrabScript>();

        activeTentaclesScripts.IsTentacleActive = true;
        activeTentaclesScripts.OnGrabEnter += HighlightTriggerPrompt;*/
    }

    private void Grab()
    {
        for (int i = 0; i < activeTentacles.Count; i++)
        {

            if (activeTentaclesScripts[i].IsHoldingPizza == false && activeTentaclesScripts[i].IsHoldingPlatter == false
                            && activeTentaclesScripts[i].CollidingSlice != null)
            {
                activeTentaclesScripts[i].IsHoldingPizza = true;

                if (activeTentaclesScripts[i].CollidingSlice.tag == "Pizza") //Grab the slice
                {
                    //Disable physics on the slice.
                    //GameObject.Destroy(tentacleGrabber.GetComponent<GrabScript>().CollidingSlice.GetComponent<Rigidbody2D>());
                    //tentacleGrabber.GetComponent<GrabScript>().CollidingSlice.GetComponent<EdgeCollider2D>().isTrigger = true;

                    //If grabbing a slice from another tentacle, make sure to reset the tentacle's bools
                    if (activeTentaclesScripts[i].CollidingSlice.transform.parent.tag == "Tentacle")
                    {
                        GrabScript sliceParentGrabScript = activeTentaclesScripts[i].CollidingSlice.transform.parent.GetComponent<GrabScript>();

                        sliceParentGrabScript.IsHoldingPizza = false;
                    }

                    activeTentaclesScripts[i].CollidingSlice.transform.parent = activeTentacleGrabbers[i].transform;
                }
            }
            else if (activeTentaclesScripts[i].IsHoldingPizza == false && activeTentaclesScripts[i].CollidingPlatter != null)
            {
                if (activeTentaclesScripts[i].CollidingPlatter.tag == "Platter") //Spin the platter
                {
                    activeTentaclesScripts[i].IsHoldingPlatter = true;

                    Quaternion platterRotation = activeTentaclesScripts[i].CollidingPlatter.transform.localRotation;

                    platterRotation.eulerAngles += new Vector3(0, 0, activeTentacleGrabbers[i].transform.localRotation.eulerAngles.z - 180) * ROTATE_SPEED * Time.deltaTime;

                    activeTentaclesScripts[i].CollidingPlatter.transform.localRotation = platterRotation;
                }
            }
        }
    }

    private void ToggleHighlightButtonPrompt(char button, bool isHighlighted)
    {
        int buttonIndex = 0;

        switch(button)
        {
            case 'A':
                buttonIndex = 0;
                break;
            case 'B':
                buttonIndex = 1;
                break;
            case 'X':
                buttonIndex = 2;
                break;
            case 'Y':
                buttonIndex = 3;
                break;
            default:
                break;
        }

        if (isHighlighted)
        {

            if (currentInputSource == InputSource.Controller)
                buttonPromptRenderers[buttonIndex].sprite = Resources.Load<Sprite>("Sprites/UI/Active_Button_" + button);
            else if (currentInputSource == InputSource.Keyboard)
                buttonPromptRenderers[buttonIndex].sprite = Resources.Load<Sprite>("Sprites/UI/Active_Key_" + button + (playerInputID + 1));
        }
        else
        {
            if (currentInputSource == InputSource.Controller)
                buttonPromptRenderers[buttonIndex].sprite = Resources.Load<Sprite>("Sprites/UI/Button_" + button);
            else if (currentInputSource == InputSource.Keyboard)
                buttonPromptRenderers[buttonIndex].sprite = Resources.Load<Sprite>("Sprites/UI/Key_" + button + (playerInputID + 1));
        }
    }

    private void HighlightTriggerPrompt()
    {
        if (activeTentacles != null)
        {
            for (int i = 0; i < activeTentacles.Count; i++)
            {
                if (activeTentaclesScripts[i].CollidingSlice != null) //Show "GRAB"
                {
                    triggerPromptTextRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_Text", "Text_Grab");

                    if (triggerPromptRenderer.enabled == false)
                    {
                        triggerPromptRenderer.enabled = true;
                        triggerPromptTextRenderer.enabled = true;
                    }
                }
                else if (activeTentaclesScripts[i].CollidingPlatter != null) //Show "SPIN"
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
        if (gameManager.PlayerInputSources[PlayerNum - 1] == null)
            Debug.Log("Player Input Source is null"); 

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
