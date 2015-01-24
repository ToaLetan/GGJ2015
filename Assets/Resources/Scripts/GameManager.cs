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
	}
	
	// Update is called once per frame
	void Update () 
    {
        inputManager.Update();
	}
}
