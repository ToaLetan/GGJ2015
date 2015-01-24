using UnityEngine;
using System.Collections;

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

        inputManager.DPad_Axis += InputTest;
	}
	
	// Update is called once per frame
	void Update () 
    {
        inputManager.Update();
	}

    private void InputTest(Vector2 axisValues)
    {
        if (axisValues.y > 0.1f)
            Debug.Log("GOING UP");
    }
}
