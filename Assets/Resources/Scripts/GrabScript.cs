using UnityEngine;
using System.Collections;

public class GrabScript : MonoBehaviour 
{
    private GameObject collidingSlice = null;
    private GameObject collidingPlatter = null;

    private bool isTentacleActive = false;
    private bool isHoldingPizza = false;
    private bool isHoldingPlatter = false;

    public GameObject CollidingSlice
    {
        get { return collidingSlice; }
        set { collidingSlice = value; }
    }

    public GameObject CollidingPlatter
    {
        get { return collidingPlatter; }
        set { collidingPlatter = value; }
    }

    public bool IsTentacleActive
    {
        get { return isTentacleActive; }
        set { isTentacleActive = value; }
    }

    public bool IsHoldingPizza
    {
        get { return isHoldingPizza; }
        set { isHoldingPizza = value; }
    }

    public bool IsHoldingPlatter
    {
        get { return isHoldingPlatter; }
        set { isHoldingPlatter = value; }
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
                collidingSlice = collisionObj.gameObject;
            }
        }
    }

    void OnCollisionStay(Collision collisionObj)
    {
        if (isTentacleActive && collidingSlice == null)
        {
            if (collisionObj.gameObject.tag == "Pizza")
            {
                collidingSlice = collisionObj.gameObject;
            }
        }
    }

    void OnCollisionExit(Collision collisionObj)
    {
        if (isTentacleActive)
        {
            if (collisionObj.gameObject.tag == "Pizza")
            {
                collidingSlice = null;
            }
        }
    }

    void OnTriggerEnter(Collider collisionObj)
    {
        if (collisionObj.gameObject.tag == "Platter")
        {
            collidingPlatter = collisionObj.gameObject;
        }
    }

    void OnTriggerStay(Collider collisionObj)
    {
        if (isTentacleActive == true && collidingPlatter == null)
        {
            if (collisionObj.gameObject.tag == "Platter")
            {
                collidingPlatter = collisionObj.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider collisionObj)
    {
        if (isTentacleActive)
        {
            if (collisionObj.gameObject.tag == "Platter")
            {
                collidingPlatter = null;
            }
        }
    }
}
