using UnityEngine;
using System.Collections;

public class GrabScript : MonoBehaviour 
{
    private GameObject collidingObject = null;

    private bool isTentacleActive = false;

    public GameObject CollidingObject
    {
        get { return collidingObject; }
        set { collidingObject = value; }
    }

    public bool IsTentacleActive
    {
        get { return isTentacleActive; }
        set { isTentacleActive = value; }
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    
    void OnCollisionEnter(Collision collisionObj)
    {
        if (isTentacleActive)
        {
            if (collisionObj.gameObject.tag == "Pizza")
            {
                collidingObject = collisionObj.gameObject;
            }
        }
    }

    void OnCollisionExit(Collision collisionObj)
    {
        if (isTentacleActive)
        {
            if (collisionObj.gameObject.tag == "Pizza")
            {
                collidingObject = null;
            }
        }
    }
}
