using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
    public int PlayerNum = 0;

    private InputManager inputManager = null;

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
