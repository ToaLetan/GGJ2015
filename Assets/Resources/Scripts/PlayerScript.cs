using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour 
{
    private const float MOVE_SPEED = 1.0f;

    public int PlayerNum = 0;

    private InputManager inputManager = null;

    private GameObject[] buttonPrompts = new GameObject[4];

    private GameObject activeTentacle = null;
    private GameObject tentacleGrabber = null;
    private Rigidbody tentacleGrabberRigidbody = null;

    private int playerControllerID = 0;

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;

        inputManager.Left_Thumbstick_Axis += ProcessMovement;
        inputManager.Button_Pressed += ProcessButtonPresses;
        inputManager.Right_Trigger_Axis += ProcessTrigger;

        playerControllerID = PlayerNum - 1;

        //Get the button prompts attached to each tentacle's first link.
        buttonPrompts[0] = gameObject.transform.FindChild("Tentacle_A").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
        buttonPrompts[1] = gameObject.transform.FindChild("Tentacle_B").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
        buttonPrompts[2] = gameObject.transform.FindChild("Tentacle_X").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
        buttonPrompts[3] = gameObject.transform.FindChild("Tentacle_Y").FindChild("Tentacle_Link_0").GetChild(0).gameObject;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void ProcessMovement(int controllerNum, Vector2 thumbstickPosition)
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

    private void ProcessButtonPresses(List<string> buttonsPressed)
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

    private void SelectTentacle(char tentacleButton)
    {
        //Get the active tentacle, the end of the tentacle, and set its script to be active.
        activeTentacle = gameObject.transform.FindChild("Tentacle_" + tentacleButton).gameObject;
        tentacleGrabber = activeTentacle.transform.FindChild("Tentacle_Grabber").gameObject;
        tentacleGrabberRigidbody = tentacleGrabber.GetComponent<Rigidbody>();

        tentacleGrabber.GetComponent<GrabScript>().IsTentacleActive = true;

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

    private void ProcessTrigger(int controllerNum, float triggerValue)
    {
        if (controllerNum == PlayerNum && triggerValue < 0.0f)
        {
            if (activeTentacle != null)
            {
                if (tentacleGrabber.GetComponent<GrabScript>().IsTentacleActive == true && tentacleGrabber.GetComponent<GrabScript>().CollidingObject != null)
                {
                    tentacleGrabber.GetComponent<GrabScript>().IsTentacleActive = false;

                    //Disable physics on the slice.
                    GameObject.Destroy(tentacleGrabber.GetComponent<GrabScript>().CollidingObject.GetComponent<Rigidbody>() );
                    tentacleGrabber.GetComponent<GrabScript>().CollidingObject.GetComponent<BoxCollider>().isTrigger = true;

                    tentacleGrabber.GetComponent<GrabScript>().CollidingObject.transform.parent = tentacleGrabber.transform;
                }
            } 
        }
    }
}
