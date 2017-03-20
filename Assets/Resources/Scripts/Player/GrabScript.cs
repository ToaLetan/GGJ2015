using UnityEngine;
using System.Collections;

public class GrabScript : MonoBehaviour 
{
    public delegate void GrabEvent();
    public GrabEvent OnGrabEnter;

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

    void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (isTentacleActive)
        {
            if (collisionObj.gameObject.tag == "Platter")
            {
                collidingPlatter = collisionObj.gameObject;

                if (OnGrabEnter != null)
                    OnGrabEnter();
            }

            if (collisionObj.gameObject.tag == "Pizza")
            {
                collidingSlice = collisionObj.gameObject;

                if (OnGrabEnter != null)
                    OnGrabEnter();
            }
        }
    }

    void OnTriggerStay2D(Collider2D collisionObj)
    {
        if (isTentacleActive)
        {
            if (collidingPlatter == null && collisionObj.gameObject.tag == "Platter")
            {
                collidingPlatter = collisionObj.gameObject;

                if (OnGrabEnter != null)
                    OnGrabEnter();
            }
            if (collidingSlice == null && collisionObj.gameObject.tag == "Pizza")
            {
                collidingSlice = collisionObj.gameObject;

                if (OnGrabEnter != null)
                    OnGrabEnter();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collisionObj)
    {
        if (isTentacleActive)
        {
            if (collisionObj.gameObject.tag == "Platter")
            {
                collidingPlatter = null;

                if (OnGrabEnter != null)
                    OnGrabEnter();
            }
            if (collisionObj.gameObject.tag == "Pizza")
            {
                collidingSlice = null;

                if (OnGrabEnter != null)
                    OnGrabEnter();
            }
        }
    }
}
