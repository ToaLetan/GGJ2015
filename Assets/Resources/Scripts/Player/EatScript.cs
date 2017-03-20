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

        thePlayer = gameObject.transform.GetComponent<PlayerScript>();

        playerNum = thePlayer.PlayerNum;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.tag == "Pizza" && thePlayer.ActiveTentacle != null)
        {
            GrabScript parentTentacleScript = collisionObj.transform.parent.GetComponent<GrabScript>();

            if (parentTentacleScript != null)
                parentTentacleScript.IsHoldingPizza = false;

            Destroy(collisionObj.gameObject);

            //thePlayer.TentacleGrabber.GetComponent<GrabScript>().IsHoldingPizza = false;

            gameManager.AdjustScore(playerNum, 1);
        }
    }
}
