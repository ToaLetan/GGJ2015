using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour 
{
    private const float MOVE_SPEED = 1.0f;
    private const float ROTATE_SPEED = 0.5f;

    public int PlayerNum = 0;

    private InputManager inputManager = null;
    private GameManager gameManager = null;

    private GameObject[] buttonPrompts = new GameObject[4];

    private GameObject triggerPrompt = null;

    private GameObject activeTentacle = null;
    private GameObject tentacleGrabber = null;
    private Rigidbody tentacleGrabberRigidbody = null;

    private Color hide = new Color(0, 0, 0, 0);

    private int playerControllerID = 0;

    public GameObject ActiveTentacle
    {
        get { return activeTentacle; }
    }

    public GameObject TentacleGrabber
    {
        get { return tentacleGrabber; }
    }

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

        inputManager.Left_Thumbstick_Axis += ProcessMovement;
        inputManager.Button_Pressed += ProcessButtonPresses;
        inputManager.Right_Trigger_Axis += ProcessTrigger;

        gameManager.GameRestart += OnGameReset;

        playerControllerID = PlayerNum - 1;

        //Get the button prompts attached to each tentacle's first link.
        buttonPrompts[0] = gameObject.transform.FindChild("Tentacle_A").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
        buttonPrompts[1] = gameObject.transform.FindChild("Tentacle_B").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
        buttonPrompts[2] = gameObject.transform.FindChild("Tentacle_X").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
        buttonPrompts[3] = gameObject.transform.FindChild("Tentacle_Y").FindChild("Tentacle_Link_0").GetChild(0).gameObject;

        triggerPrompt = GameObject.Find("RT_Prompt_Player_" + PlayerNum);
        triggerPrompt.transform.GetComponent<SpriteRenderer>().color = hide;
        triggerPrompt.transform.GetChild(0).GetComponent<SpriteRenderer>().color = hide;
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    private void ProcessMovement(int controllerNum, Vector2 thumbstickPosition)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            if (controllerNum == PlayerNum && activeTentacle != null)
            {
                if (thumbstickPosition != Vector2.zero)
                {
                    tentacleGrabber.transform.position += new Vector3(thumbstickPosition.x, thumbstickPosition.y, 0) * MOVE_SPEED * Time.deltaTime;
                }
                else
                    tentacleGrabberRigidbody.angularVelocity = Vector2.zero;
            }
        }
    }

    private void ProcessButtonPresses(List<string> buttonsPressed)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            if (buttonsPressed.Contains(inputManager.ControllerArray[playerControllerID].buttonA))
            {
                SelectTentacle('A');
            }
            if (buttonsPressed.Contains(inputManager.ControllerArray[playerControllerID].buttonB))
            {
                SelectTentacle('B');
            }
            if (buttonsPressed.Contains(inputManager.ControllerArray[playerControllerID].buttonX))
            {
                SelectTentacle('X');
            }
            if (buttonsPressed.Contains(inputManager.ControllerArray[playerControllerID].buttonY))
            {
                SelectTentacle('Y');
            }
        }
        if (buttonsPressed.Contains(inputManager.ControllerArray[playerControllerID].startButton)) //Show/hide the pause menu.
        {
            if(gameManager.IsGamePaused == false || gameManager.IsGameOver == true)
                gameManager.ShowHidePauseMenu(PlayerNum, true);
            else if(gameManager.IsGamePaused == true && gameManager.IsPlayingIntro == false)
                gameManager.ShowHidePauseMenu(PlayerNum, false);
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
        tentacleGrabberRigidbody = tentacleGrabber.GetComponent<Rigidbody>();

        tentacleGrabber.GetComponent<GrabScript>().IsTentacleActive = true;
        tentacleGrabber.GetComponent<GrabScript>().OnGrabEnter += HighlightTriggerPrompt;

        //Highlight the selected button prompt.
        HighlightButtonPrompt(tentacleButton);
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

        buttonPrompts[currentActiveButtonIndex].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Active_Button_" + button);

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
                    buttonPrompts[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Button_" + currentButton);
            }
        }
    }

    private void HighlightTriggerPrompt()
    {
        if (activeTentacle != null)
        {
            if (tentacleGrabber.GetComponent<GrabScript>().CollidingSlice != null) //Show "GRAB"
            {
                triggerPrompt.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Grab");

                if (triggerPrompt.transform.GetComponent<SpriteRenderer>().color == hide)
                {
                    triggerPrompt.transform.GetComponent<SpriteRenderer>().color = Color.white;
                    triggerPrompt.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            else if (tentacleGrabber.GetComponent<GrabScript>().CollidingPlatter != null) //Show "SPIN"
            {
                triggerPrompt.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Spin");

                if (triggerPrompt.transform.GetComponent<SpriteRenderer>().color == hide)
                {
                    triggerPrompt.transform.GetComponent<SpriteRenderer>().color = Color.white;
                    triggerPrompt.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            else
            {
                triggerPrompt.transform.GetComponent<SpriteRenderer>().color = hide;
                triggerPrompt.transform.GetChild(0).GetComponent<SpriteRenderer>().color = hide;
            }
        }
    }

    private void ProcessTrigger(int controllerNum, float triggerValue)
    {
        if (gameManager.IsGamePaused == false && gameManager.IsGameOver == false)
        {
            if (controllerNum == PlayerNum && triggerValue < 0.0f)
            {
                if (activeTentacle != null)
                {
                    if (tentacleGrabber.GetComponent<GrabScript>().IsHoldingPizza == false && tentacleGrabber.GetComponent<GrabScript>().IsHoldingPlatter == false
                        && tentacleGrabber.GetComponent<GrabScript>().CollidingSlice != null)
                    {
                        tentacleGrabber.GetComponent<GrabScript>().IsHoldingPizza = true;

                        if (tentacleGrabber.GetComponent<GrabScript>().CollidingSlice.tag == "Pizza") //Grab the slice
                        {
                            //Disable physics on the slice.
                            GameObject.Destroy(tentacleGrabber.GetComponent<GrabScript>().CollidingSlice.GetComponent<Rigidbody>());
                            tentacleGrabber.GetComponent<GrabScript>().CollidingSlice.GetComponent<BoxCollider>().isTrigger = true;

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
                    //Highlight RT
                    triggerPrompt.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Active_Trigger_Right");

                }
            }
            if (controllerNum == PlayerNum && triggerValue >= 0.0f) //Let go of the platter if previously holding it.
            {
                if (activeTentacle != null)
                {
                    if (tentacleGrabber.GetComponent<GrabScript>().IsHoldingPlatter == true)
                        tentacleGrabber.GetComponent<GrabScript>().IsHoldingPlatter = false;

                    //Highlight RT
                    triggerPrompt.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Trigger_Right");
                }
            }
        }
    }

    private void OnGameReset()
    {
        inputManager.Left_Thumbstick_Axis -= ProcessMovement;
        inputManager.Button_Pressed -= ProcessButtonPresses;
        inputManager.Right_Trigger_Axis -= ProcessTrigger;
    }
}
