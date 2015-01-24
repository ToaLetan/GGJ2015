using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    private InputManager inputManager = null;

    public InputManager GameInputManager
    {
        get { return inputManager; }
    }

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;

        inputManager.Left_Thumbstick_Axis += InputTest;
        inputManager.Button_Pressed += ButtonTest;
	}
	
	// Update is called once per frame
	void Update () 
    {
        inputManager.Update();
	}

    private void InputTest(int controllerNum, Vector2 axisValues)
    {
        if (axisValues.y > 0.1f)
            Debug.Log("GOING UP " + controllerNum);
    }

    private void ButtonTest(List<string> buttonsHeld)
    {
        if (buttonsHeld.Contains(inputManager.ControllerArray[0].buttonA) )
            Debug.Log(inputManager.ControllerArray[0].buttonA);
        if (buttonsHeld.Contains(inputManager.ControllerArray[1].buttonA))
            Debug.Log(inputManager.ControllerArray[1].buttonA);
    }
}
