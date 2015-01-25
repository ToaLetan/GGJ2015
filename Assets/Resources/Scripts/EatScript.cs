using UnityEngine;
using System.Collections;

public class EatScript : MonoBehaviour 
{
    private GameManager gameManager = null;

    private PlayerScript thePlayer = null;

    private int playerNum = 0;

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

        thePlayer = gameObject.transform.parent.GetComponent<PlayerScript>();

        playerNum = thePlayer.PlayerNum;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter(Collider collisionObj)
    {
        if (collisionObj.tag == "Pizza" && thePlayer.ActiveTentacle != null)
        {
            Destroy(collisionObj.gameObject);

            thePlayer.TentacleGrabber.GetComponent<GrabScript>().IsHoldingPizza = false;

            gameManager.AdjustScore(playerNum, 1);
        }
    }
}
